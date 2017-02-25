using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public static class Scripts {
        public const string ContentType = "application/javascript";
        public const string TagFormat = "<script src=\"{0}\"></script>";

        private const string PlaceHolder = ";\r\n";

        public static IBundle CreateScriptBundle(this IBundleProvider bundleProvider) {
            return new Bundle(bundleProvider.Context, ContentType, PlaceHolder, TagFormat, new JavaScriptContentTransformer());
        }
    }
}
