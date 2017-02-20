using System;
using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public class JavaScriptContentBundler : IContentBundler {
        public static readonly IContentBundler Instance = new JavaScriptContentBundler();

        public const string ContentType = "application/javascript";

        string IContentBundler.ContentType { get; } = ContentType;

        string IContentBundler.GenerateTag(string src) {
            return $"<script src=\"{src}\"></script>";
        }

        string IContentBundler.Placeholder { get; } = ";" + Environment.NewLine;

        IContentTransformer IContentBundler.CreateTransformer() => new JavaScriptContentTransformer();
    }
}