using System;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public class Bundle : IBundle {
        public string VirtualPath { get; }
        public string ContentType { get; }

        private readonly Container _container;
        private readonly IContentBundler _contentBundler;

        public Bundle(string virtualPath, IContentBundler contentBundler) {
            VirtualPath = virtualPath;
            ContentType = contentBundler.ContentType;

            _contentBundler = contentBundler;
            _container = new Container(contentBundler.Placeholder);
        }

        public bool Append(string identifier, string content) {
            if (string.IsNullOrWhiteSpace(content)) {
                return false;
            }

            if (_container.Exists(identifier)) {
                return true;
            }

            using (var transformer = _contentBundler.CreateTransformer()) {
                string processedContent;
                if (!transformer.Process(content, out processedContent)) {
                    return false;
                }

                _container.Append(identifier, processedContent);
            }

            return true;
        }

        public DateTime LastModification => _container.LastModification;
        public int Version => _container.Version;
        public string Content => _container.Content;

        public string GenerateTag(string url) {
            return _contentBundler.GenerateTag(url);
        }
    }
}
