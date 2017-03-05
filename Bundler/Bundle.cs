using System;
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
        }

        protected virtual bool ProcessContent(IFileContent fileContent) {
            return ContentTransformers.All(t => t.Process(Context, fileContent));
        }

        public bool Include(string virtualFile, string content) {
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (_container.Exists(virtualFile)) {
                return true;
            }

            var fileContent = new FileContent(virtualFile, content);
            var processResult = ProcessContent(fileContent);
            if (!processResult) {
                if (!Context.FallbackOnError) {
                    return false;
                }

                _container.Append(virtualFile, content);
                return true;
            }

            _container.Append(virtualFile, fileContent.Content);
            return true;
        }

        public IBundleResponse GetResponse() {
            return new BundleResponse(ContentType, _container.ContentHash, _container.LastModification, _container.Content, _container.GetFiles());
        }

        private class FileContent : IFileContent {
            public FileContent(string virtualFile, string inputContent) {
                VirtualFile = virtualFile;
                Content = inputContent;
            }

            public string VirtualFile { get; }
            public string Content { get; set; }
        }
    }
}
