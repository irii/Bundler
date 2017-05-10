using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public sealed class JavaScriptRenderer : IBundleRenderer {
        private readonly string _tagFormat;

        public static readonly IBundleRenderer Instance = new JavaScriptRenderer(async: false);
        public static readonly IBundleRenderer InstanceAsync = new JavaScriptRenderer(async: true);

        public JavaScriptRenderer(bool async) {
            _tagFormat = async
                ? TagFormat
                : TagFormatAsync;
        }

        public const string TagFormat = "<script src=\"{0}\"></script>";
        public const string TagFormatAsync = "<script src=\"{0}\" async></script>";
        public const string ContentType = "application/javascript";
        public const string PlaceHolder = ";\r\n";

        string IBundleRenderer.Render(string url) {
            return string.Format(_tagFormat, url);
        }

        string IBundleRenderer.ContentType { get; } = ContentType;

        string IBundleRenderer.Concat(IEnumerable<string> processedItems) {
            return string.Join(PlaceHolder, processedItems);
        }
    }
}
