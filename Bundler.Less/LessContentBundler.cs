using System;
using Bundler.Infrastructure;

namespace Bundler.Less {
    public class LessContentBundler : IContentBundler {
        public static readonly IContentBundler Instance = new LessContentBundler();

        /// <summary>
        /// Less input gets converted into css
        /// </summary>
        public const string ContentType = "text/css";

        string IContentBundler.ContentType { get; } = ContentType;

        string IContentBundler.GenerateTag(string src) {
            return $"<link rel=\"stylesheet\" href=\"{src}\">";
        }

        string IContentBundler.Placeholder { get; } = Environment.NewLine;

        IContentTransformer IContentBundler.CreateTransformer() => new LessContentTransformer();
    }
}