using Bundler.Infrastructure;

namespace Bundler.Css {
    public static class CssStyles {
        public const string ContentType = "text/css";
        public const string TagFormat = "<link rel=\"stylesheet\" href=\"{0}\">";

        private const string PlaceHolder = "\r\n";

        public static IBundle CreateCssBundle(this IBundleProvider bundleProvider) {
            return new Bundle(bundleProvider.Context, ContentType, PlaceHolder, TagFormat, new CssContentTransformer());
        }
    }
}