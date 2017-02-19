namespace Bundler.Infrastructure {
    public class BundleInfo {
        public readonly IContentBundler ContentBundler;

        public readonly string VirtualPath;
        public readonly string BundleKey;

        public readonly Container Container = new Container();

        public BundleInfo(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            BundleKey = bundleKey;
            VirtualPath = virtualPath;
            ContentBundler = contentBundler;
        }
    }
}
