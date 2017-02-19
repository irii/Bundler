using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.Css {
    public class CssContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(string inputContent, out string outputContent) {
            if (string.IsNullOrWhiteSpace(inputContent)) {
                outputContent = string.Empty;
                return true;
            }

            var minifier = new Minifier();
            outputContent = minifier.MinifyStyleSheet(inputContent, new CssSettings { CommentMode = CssComment.None })?.Trim() ?? string.Empty;
            return true;
        }
    }
}