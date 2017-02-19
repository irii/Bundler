using System;
using Bundler.Infrastructure;

namespace Bundler.Css {
    public class CssContentBundler : IContentBundler {
        public static readonly IContentBundler Instance = new CssContentBundler();

        public const string ContentType = "text/css";

        string IContentBundler.ContentType { get; } = ContentType;

        string IContentBundler.GenerateTag(string src) {
            return $"<link rel=\"stylesheet\" href=\"{src}\">";
        }

        string IContentBundler.Placeholder { get; } = Environment.NewLine;

        IContentTransformer IContentBundler.CreateTransformer() => new CssContentTransformer();
    }
}