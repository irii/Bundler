using Bundler.Infrastructure;
using Bundler.Infrastructure.Transform;

namespace Bundler.Css {
    public static class CssStyles {
        public static BundleBuilder CreateCssBundle(this IBundleProvider bundleProvider, params IBundleContentTransformer[] additionalContentTransformers) {
            return new BundleBuilder(bundleProvider.Context, CssRenderer.Instance).AddContentTransformer(new CssBundleContentTransformer());
        }
    }
}