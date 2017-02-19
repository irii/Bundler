using Bundler.Infrastructure;
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
            _contentBundler = contentBundler;
            ContentType = contentBundler.ContentType;

            _container = new Container(contentBundler.Placeholder);
        }

        public bool Add(string identifier, string content) {
            if (_container.Exists(identifier)) {
                return true;
            }

            if (string.IsNullOrWhiteSpace(content)) {
                return false;
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

        public string Get() => _container.Get();

        public string Render() {
            return _contentBundler.GenerateTag(string.Concat(VirtualPath, "?v=", _container.GetVersion()));
        }
    }
}
