using Bundler.Infrastructure;

namespace Bundler.Less {
    public static class LessStyles {
        public const string ContentType = "text/css";
        public const string TagFormat = "<link rel=\"stylesheet\" href=\"{0}\">";

        private const string PlaceHolder = "\r\n";

        public static BundleBuilder CreateLessBundle(this IBundleProvider bundleProvider) {
            return new BundleBuilder(bundleProvider.Context, ContentType, PlaceHolder, TagFormat).AddContentTransformer(new LessBundleContentTransformer());
        }
    }
}
