using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.Css {
    public class CssContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundleContext bundleContext, string inputContent, out string outputContent) {
            if (!bundleContext.Optimization || string.IsNullOrWhiteSpace(inputContent)) {
                outputContent = inputContent?.Trim() ?? string.Empty;
                return true;
            }

            var minifier = new Minifier();
            outputContent = minifier.MinifyStyleSheet(inputContent, new CssSettings { CommentMode = CssComment.None })?.Trim() ?? string.Empty;
            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(outputContent);
        }
    }
}