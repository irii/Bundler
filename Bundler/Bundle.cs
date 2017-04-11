﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public class Bundle : IBundle {
        private const string Tag = nameof(Bundle);

        protected readonly IBundleContentTransformer[] BundleContentTransformers;

        public string ContentType { get; }
        public string TagFormat { get; }
        public IBundleContext Context { get; }

        private readonly Container _container;

        public Bundle(IBundleContext bundleContext, string contentType, string placeholder, string tagFormat, params IBundleContentTransformer[] contentTransformers) {
            Context = bundleContext;
            BundleContentTransformers = contentTransformers?.ToArray() ?? new IBundleContentTransformer[0];
            ContentType = contentType;
            TagFormat = tagFormat;

            _container = new Container(placeholder, contentType);

            ChangeHandler = path => {
                if (Context.Configuration.AutoRefresh) {
                    Refresh();
                }
            };
        }

        private bool ProcessContent(BundleContentTransform bundleContentTransformResult) {
            return BundleContentTransformers.All(t => t.Process(this, bundleContentTransformResult));
        }

        public bool Include(ISource source) => IncludeInternal(source, _container);

        private bool IncludeInternal(ISource source, Container container) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (container.Exists(source.VirtualFile)) {
                Context.Diagnostic.Log(LogLevel.Debug, Tag, nameof(IncludeInternal), $"{source.VirtualFile} is already added to the bundle.");
                return true;
            }

            var transformResult = new BundleContentTransform(source.VirtualFile, source.Get());
            if (transformResult.Content == null) {
                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(IncludeInternal), $"Failed to read content from  {source.VirtualFile}");
                return false;
            }

            var inputContent = transformResult.Content;

            var processResult = ProcessContent(transformResult);
            if (!processResult) {
                var errors = string.Join("; ", transformResult.Errors);

                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(IncludeInternal),$"Failed to Process {source.VirtualFile}: {errors}");
                if (!Context.Configuration.FallbackOnError) {
                    return false;
                }

                Context.Diagnostic.Log(LogLevel.Error, Tag, nameof(IncludeInternal), $"Using fallback behaviour for {source.VirtualFile}");
            } else {
                inputContent = transformResult.Content;
            }

            // Returns false only if file was already added.
            if (!container.Append(source, inputContent)) {
                return true;
            }

            Context.Diagnostic.Log(LogLevel.Debug, Tag, nameof(IncludeInternal), $"{source.VirtualFile} added to bundle.");

            // Register for auto refresh
            if (source.IsWatchable && Context.VirtualPathProvider.FileExists(source.VirtualFile)) {
                Context.Watcher.Watch(source.VirtualFile, ChangeHandler);   
            }

            return true;
        }


        public bool Refresh() {
            return _container.Refresh((current, newContainer) => {
                foreach (var file in current.Files) {
                    var source = current.Sources[file.Value];
                    if (!IncludeInternal(source, newContainer)) {
                        Context.Diagnostic.Log(LogLevel.Info, Tag, nameof(Refresh), $"Failed to refresh bundle.");
                        return false;
                    }
                }

                Context.Diagnostic.Log(LogLevel.Info, Tag, nameof(Refresh), $"Bundle refreshed.");
                return true;
            });
        }

        public IBundleResponse GetResponse() {
            return new BundleResponse(ContentType, _container.ContentHash, _container.LastModification, _container.Content, _container.Current.Files, new Dictionary<string, string>(0));
        }

        public void Dispose() {
            foreach (var file in _container.Current.Files) {
                var source = _container.Current.Sources[file.Value];
                Context.Watcher.Unwatch(source.VirtualFile, ChangeHandler);
            }
            
            // Make conatiner empty to lose all dependencies
            _container.Refresh((tuple, container) => true);
        }

        public FileChangedDelegate ChangeHandler { get; }

        public string Render(string url) {
            return string.Format(TagFormat, url);
        }
    }
}
