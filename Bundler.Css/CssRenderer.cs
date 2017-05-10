using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler.Css {
    public sealed class CssRenderer : IBundleRenderer {
        public static readonly IBundleRenderer Instance = new CssRenderer();

        public const string ContentType = "text/css";
        public const string TagFormat = "<link rel=\"stylesheet\" href=\"{0}\">";
        private const string PlaceHolder = "\r\n";


        string IBundleRenderer.Render(string url) {
            return string.Format(TagFormat, url);
        }

        string IBundleRenderer.ContentType { get; } = ContentType;

        string IBundleRenderer.Concat(IEnumerable<string> processedItems) {
            return string.Join(PlaceHolder, processedItems);
        }
    }
}
