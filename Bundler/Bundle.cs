using System;
using System.Collections.Generic;
using System.Linq;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public class Bundle : IBundle {
        protected readonly IContentTransformer[] ContentTransformers;

        public string ContentType { get; }
        public string TagFormat { get; }
        public IBundleContext Context { get; }

        private readonly Container _container;

        public Bundle(IBundleContext bundleContext, string contentType, string placeholder, string tagFormat, params IContentTransformer[] transformers) {
            Context = bundleContext;
            ContentTransformers = transformers?.ToArray() ?? new IContentTransformer[0];
            ContentType = contentType;
            TagFormat = tagFormat;

            _container = new Container(placeholder);

            ChangeHandler = path => {
                if (Context.AutoRefresh) {
                    Refresh();
                }
            };
        }

        private bool ProcessContent(IContentTransform contentTransform) {
            return ContentTransformers.All(t => t.Process(this, contentTransform));
        }

        public bool Include(IContentSource contentSource) => IncludeInternal(contentSource, _container);

        private bool IncludeInternal(IContentSource contentSource, Container container) {
            if (contentSource == null) throw new ArgumentNullException(nameof(contentSource));
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (container.Exists(contentSource.VirtualFile)) {
                return true;
            }

            var fileContent = new ContentTransform(contentSource.VirtualFile, contentSource.Get());
            if (fileContent.Content == null) {
                return false;
            }

            var inputContent = fileContent.Content;

            var processResult = ProcessContent(fileContent);
            if (!processResult) {
                if (!Context.FallbackOnError) {
                    return false;
                }

            } else {
                inputContent = fileContent.Content;
            }

            if (!container.Append(contentSource, inputContent)) {
                return true;
            }

            // Register for auto refresh
            if (contentSource.IsWatchable && Context.VirtualPathProvider.FileExists(contentSource.VirtualFile)) {
                Context.Watcher.Watch(contentSource.VirtualFile, ChangeHandler);   
            }

            return true;
        }


        public bool Refresh() {
            return _container.Refresh((current, newContainer) => {
                foreach (var file in current.Files) {
                    var source = current.Sources[file.Value];
                    IncludeInternal(source, newContainer);
                }

                return true;
            });
        }

        public IBundleResponse GetResponse() {
            return new BundleResponse(ContentType, _container.ContentHash, _container.LastModification, _container.Content, _container.Current.Files);
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

        private class ContentTransform : IContentTransform {
            private readonly List<string> _errors = new List<string>(0);

            public ContentTransform(string virtualFile, string content) {
                VirtualPath = virtualFile;
                Content = content;
            }

            public string Content { get; set; }
            public string VirtualPath { get; }
            public IReadOnlyCollection<string> Errors => _errors;

            public void AddError(string logMessage) {
                if (!_errors.Contains(logMessage)) {
                    _errors.Add(logMessage);
                }
            }
        }
    }
}
