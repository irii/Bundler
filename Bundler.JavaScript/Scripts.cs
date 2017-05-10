using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public static class Scripts {
        public static BundleBuilder CreateScriptBundle(this IBundleProvider bundleProvider, bool async = false) {
            return new BundleBuilder(bundleProvider.Context, async ? JavaScriptRenderer.InstanceAsync : JavaScriptRenderer.Instance).AddContentTransformer(new JavaScriptBundleContentTransformer());
        }
    }
}
