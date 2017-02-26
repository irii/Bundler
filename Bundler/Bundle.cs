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
        
        protected virtual string ProcessContent(string content) {
            return ContentTransformers.Any(t => !t.Process(Context, content, out content)) ? null : content;
        }
        
        public bool Include(string virtualFile, string content) {
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));
            if (content == null) throw new ArgumentNullException(nameof(content));

            if (_container.Exists(virtualFile)) {
                return true;
            }

            var transformedContent = ProcessContent(content);
            if (transformedContent == null) {
                if (!Context.FallbackOnError) {
                    return false;
                }

                transformedContent = content;
            }
            
            _container.Append(virtualFile, transformedContent);
            return true;
        }

        public IBundleResponse GetResponse() {
            return new BundleResponse(ContentType, _container.Version, _container.LastModification, _container.Content, _container.GetFiles());
        }
    }
}
