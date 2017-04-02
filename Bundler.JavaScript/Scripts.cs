using System.Linq;
using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public static class Scripts {
        public const string ContentType = "application/javascript";
        public const string TagFormat = "<script src=\"{0}\"></script>";
        public const string TagFormatAsync = "<script src=\"{0}\" async></script>";

        private const string PlaceHolder = ";\r\n";

        public static IBundle CreateScriptBundle(this IBundleProvider bundleProvider, bool async = false, params IBundleContentTransformer[] additionalContentTransformers) {
            return new Bundle(bundleProvider.Context, ContentType, PlaceHolder, async ? TagFormatAsync : TagFormat, new[] { new JavaScriptBundleContentTransformer() }.Union(additionalContentTransformers).ToArray());
        }
    }
}
