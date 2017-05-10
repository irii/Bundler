using Bundler.Infrastructure;
using Bundler.Infrastructure.Transform;

namespace Bundler.Helper {
    public static class BundleHelper {
        public static Bundle CreateBundle(this IBundleProvider bundleProvider, IBundleRenderer bundleRenderer, params IBundleContentTransformer[] transformers) {
            return new Bundle(bundleProvider.Context, bundleRenderer, transformers);
        }
    }
}
