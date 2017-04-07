using System;
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
                return true;
            }

            var fileContent = new BundleContentTransform(source.VirtualFile, source.Get());
            if (fileContent.Content == null) {
                return false;
            }

            var inputContent = fileContent.Content;

            var processResult = ProcessContent(fileContent);
            if (!processResult) {
                if (!Context.Configuration.FallbackOnError) {
                    return false;
                }

            } else {
                inputContent = fileContent.Content;
            }

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
                    IncludeInternal(source, newContainer);
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
