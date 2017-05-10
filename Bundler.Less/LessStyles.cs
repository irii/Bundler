using Bundler.Infrastructure;

namespace Bundler.Less {
    public static class LessStyles {
        public static BundleBuilder CreateLessBundle(this IBundleProvider bundleProvider) {
            return new BundleBuilder(bundleProvider.Context, LessRenderer.Instance).AddContentTransformer(new LessBundleContentTransformer());
        }
    }
}
