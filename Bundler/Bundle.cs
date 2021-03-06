﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bundler.Comparers;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;
using Bundler.Infrastructure.Transform;

namespace Bundler {
    public class Bundle : IBundle {
        private readonly IBundleRenderer _bundleRenderer;
        private const string Tag = nameof(Bundle);
        
        protected readonly IBundleContentTransformer[] BundleContentTransformers;

        public IBundleContext Context { get; }

        private BundleState _bundleState;
        private readonly object _bundleStateWriteLock = new object();

        private bool TryUpdateBundleState(Func<BundleState> getBundleState) {
            lock (_bundleStateWriteLock) {
                var newBundleState = getBundleState();
                if (newBundleState == null) {
                    return false;
                }

                var oldBundleSate = _bundleState;

                // Set new bundle state
                Interlocked.Exchange(ref _bundleState, newBundleState);

                // Configurate new bundle state
                ConfigurateBundleStateUpdate(oldBundleSate, newBundleState);

                return true;
            }
        }

        public Bundle(IBundleContext bundleContext, IBundleRenderer bundleRenderer, IBundleContentTransformer[] contentTransformers) {
            _bundleRenderer = bundleRenderer;
            Context = bundleContext;
            BundleContentTransformers = contentTransformers?.ToArray() ?? new IBundleContentTransformer[0];

            _bundleState = BundleState.CreateEmpty(_bundleRenderer.ContentType);
        }

        private void ChangeHandler(string virtualPath) {
            // TODO: Wait for more events
            if (Context.Configuration.Get(BundlingConfiguration.AutoRefresh)) {
                Refresh();
            }
        }

        public bool Add(params ISource[] sources) {
            if (sources.All(x => _bundleState.Sources.Contains(x, SourceEqualityComparer.Default))) {
                return true;
            }
            
            var success = TryUpdateBundleState(() => {
                var missingSources = sources
                    .Where(x => !_bundleState.Sources.Contains(x, SourceEqualityComparer.Default))
                    .ToList();

                if (missingSources.Count == 0) {
                    // Don't update reuse current bundle state.
                    return _bundleState;
                }
                
                return CreateBundleState(_bundleState.Sources.Union(missingSources).ToList());
            });

            if (success) {
                Context.Diagnostic.Log(LogLevel.Debug, Tag, nameof(Add), $"Added new sources {string.Join("; ", sources.Select(x => x.Identifier))}");
            } else {
                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(Add), $"Failed to add new sources {string.Join("; ", sources.Select(x => x.Identifier))}");
            }

            return success;
        }

        private BundleState CreateBundleState(IReadOnlyCollection<ISource> sources) {
            var items = new HashSet<ISourceItem>(SourceItemEqualityComparer.Default);
            var watchPaths = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var containerSource in sources) {
                if (!containerSource.AddItems(Context, items, watchPaths)) {
                    return null;
                }
            }

            var transformResults = items.Select(TransformItem).ToArray();
            if (transformResults.Any(x => x == null || x.Errors.Count > 0)) {
                return null;
            }

            // Join watch paths
            watchPaths.UnionWith(transformResults.SelectMany(x => x.WatchPaths));

            var responseContent = _bundleRenderer.Concat(transformResults.Select(x => x.Content));
            var fileRespones = transformResults
                .Select(x => new BundleFileResponse(x.VirtualFile, _bundleRenderer.ContentType, GetContentHash(x.Content), x.Content, DateTime.UtcNow))
                .ToDictionary(x => x.VirtualFile, x => (IBundleContentResponse)x, StringComparer.InvariantCultureIgnoreCase);

            var bundleResponse = new BundleResponse(_bundleRenderer.ContentType, GetContentHash(responseContent), DateTime.UtcNow, responseContent, fileRespones, new Dictionary<string, string>(0));

            var bundleState = new BundleState(sources.ToArray(), watchPaths.ToArray(), bundleResponse);
            return bundleState;
        }

        private void ConfigurateBundleStateUpdate(BundleState oldBundleState, BundleState newBundleState) {
            foreach (var bundleStateWatch in oldBundleState.Watches) {
                if (newBundleState.Watches.Contains(bundleStateWatch, StringComparer.InvariantCultureIgnoreCase)) {
                    continue;
                }

                Context.Watcher.Unwatch(bundleStateWatch, ChangeHandler);
            }

            foreach (var bundleStateWatch in newBundleState.Watches) {
                if (oldBundleState.Watches.Contains(bundleStateWatch, StringComparer.InvariantCultureIgnoreCase)) {
                    continue;
                }

                Context.Watcher.Watch(bundleStateWatch, ChangeHandler);
            }
        }

        private BundleTransformItem TransformItem(ISourceItem sourceItem) {
            if (sourceItem == null) throw new ArgumentNullException(nameof(sourceItem));

            var inputContent = sourceItem.Get();
            var bundleTransformItem = new BundleTransformItem(sourceItem.VirtualFile, inputContent);
            if (bundleTransformItem.Content == null) {
                bundleTransformItem.Content = string.Empty;
                bundleTransformItem.Errors.Add("Failed to get content");
                return bundleTransformItem;
            }

            if (BundleContentTransformers.All(t => t.Process(this, bundleTransformItem))) {
                return bundleTransformItem;
            }

            var errors = string.Join("; ", bundleTransformItem.Errors);

            Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(TransformItem),$"Failed to Process {sourceItem.VirtualFile}: {errors}");
            if (!Context.Configuration.Get(BundlingConfiguration.FallbackOnError) || !bundleTransformItem.CanUseFallback) {
                return null;
            }

            Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(TransformItem), $"Using fallback behaviour for {sourceItem.VirtualFile}");

            bundleTransformItem.Content = sourceItem.Get();
            if (bundleTransformItem.Content == null) {
                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(TransformItem), $"Using fallback failed for {sourceItem.VirtualFile}");
                return null;
            }

            bundleTransformItem.Errors.Clear();
            return bundleTransformItem;
        }


        public bool Refresh() {
            var result = TryUpdateBundleState(() => CreateBundleState(_bundleState.Sources));
            if (result) {
                Context.Diagnostic.Log(LogLevel.Debug, Tag, nameof(Refresh), "Refresh success.");
            } else {
                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(Refresh), "Refresh failed.");
            }

            return result;
        }

        public IBundleResponse GetResponse() => _bundleState.Response;

        public void Dispose() {
            // Make conatiner empty to lose all dependencies
            TryUpdateBundleState(() => BundleState.CreateEmpty(_bundleRenderer.ContentType));
        }

        public string Render(string url) {
            return _bundleRenderer.Render(url);
        }

        private class BundleState {
            public BundleState(IReadOnlyCollection<ISource> sources, IReadOnlyCollection<string> watches, IBundleResponse response) {
                Sources = sources;
                Watches = watches;
                Response = response;
            }

            public IReadOnlyCollection<ISource> Sources { get; }
            public IReadOnlyCollection<string> Watches { get; }
            public IBundleResponse Response { get; }

            public static BundleState CreateEmpty(string contentType) => new BundleState(new ISource[0], new string[0], new BundleResponse(contentType, string.Empty.GetHashCode().ToString(),
                DateTime.UtcNow, string.Empty, new Dictionary<string, IBundleContentResponse>(), new Dictionary<string, string>()));
        }

        private static string GetContentHash(string input) {
            using (var sha256 = SHA256.Create()) {
                var inputBytes = Encoding.Unicode.GetBytes(input);
                var result = Convert.ToBase64String(sha256.ComputeHash(inputBytes))
                    .Replace("+", "-").Replace("/", "_");

                var paddingCount = 0;
                var indexOfPadding = result.IndexOf('=');
                if (indexOfPadding >= 0) {
                    paddingCount = result.Length - indexOfPadding;
                }

                return result.Remove(result.Length - paddingCount) + "_" + paddingCount;
            }
        }
    }
}
