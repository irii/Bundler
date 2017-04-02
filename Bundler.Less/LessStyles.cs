using System.Linq;
using Bundler.Infrastructure;

namespace Bundler.Less {
    public static class LessStyles {
        public const string ContentType = "text/css";
        public const string TagFormat = "<link rel=\"stylesheet\" href=\"{0}\">";

        private const string PlaceHolder = "\r\n";

        public static IBundle CreateLessBundle(this IBundleProvider bundleProvider, params IBundleContentTransformer[] additionalContentTransformers) {
            return new Bundle(bundleProvider.Context, ContentType, PlaceHolder, TagFormat, new[] { new LessBundleContentTransformer() }.Union(additionalContentTransformers).ToArray());
        }
    }
}
