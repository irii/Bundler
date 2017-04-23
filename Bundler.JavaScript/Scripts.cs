using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public static class Scripts {
        public const string ContentType = "application/javascript";
        public const string TagFormat = "<script src=\"{0}\"></script>";
        public const string TagFormatAsync = "<script src=\"{0}\" async></script>";

        public const string PlaceHolder = ";\r\n";

        public static BundleBuilder CreateScriptBundle(this IBundleProvider bundleProvider, bool async = false) {
            return new BundleBuilder(bundleProvider.Context, ContentType, PlaceHolder, async ? TagFormatAsync : TagFormat).AddContentTransformer(new JavaScriptBundleContentTransformer());
        }
    }
}
