﻿using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public class Bundle {
        public readonly string VirtualPath;
        public readonly string BundleKey;
        public readonly string ContentType;

        private readonly Container _container;
        private readonly IContentBundler _contentBundler;

        public Bundle(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            BundleKey = bundleKey;
            VirtualPath = virtualPath;
            ContentType = contentBundler.ContentType;

            _contentBundler = contentBundler;
            _container = new Container(contentBundler.Placeholder);
        }

        public bool Add(string identifier, string content) {
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

        public int Version => _container.Version;

        public string Content => _container.Content;

        public string Render() {
            return _contentBundler.GenerateTag(string.Concat(VirtualPath, "?v=", _container.Version));
        }
    }
}
