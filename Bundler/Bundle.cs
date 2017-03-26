﻿using System;
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
            ChangeHandler = path => Refresh();
        }

        private bool ProcessContent(IFileContent fileContent) {
            return ContentTransformers.All(t => t.Process(this, fileContent));
        }

        public bool Include(IContentSource contentSource) => IncludeInternal(contentSource, _container);

        private bool IncludeInternal(IContentSource contentSource, Container container) {
            if (contentSource == null) throw new ArgumentNullException(nameof(contentSource));
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (container.Exists(contentSource.VirtualFile)) {
                return true;
            }

            var fileContent = new FileContent(contentSource.VirtualFile, contentSource.Get());
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

            container.Append(contentSource, inputContent);

            // Register for auto refresh
            contentSource.OnSourceChanged += (sender, args) => {
                if (Context.AutoRefresh) {
                    Refresh();
                }
            };

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
            // Make conatiner empty to lose all dependencies
            _container.Refresh((tuple, container) => true);
        }

        public FileChangedDelegate ChangeHandler { get; }

        private class FileContent : IFileContent {
            public FileContent(string virtualFile, string content) {
                VirtualFile = virtualFile;
                Content = content;
            }

            public string VirtualFile { get; }
            public string Content { get; set; }
        }
    }
}
