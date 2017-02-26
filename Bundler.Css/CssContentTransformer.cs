using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.Css {
    public class CssContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundleContext bundleContext, IFileContent fileContent) {
            if (!bundleContext.Optimization || string.IsNullOrWhiteSpace(fileContent.Content)) {
                fileContent.Content = fileContent.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            fileContent.Content = minifier.MinifyStyleSheet(fileContent.Content, new CssSettings { CommentMode = CssComment.None })?.Trim() ?? string.Empty;
            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(fileContent.Content);
        }
    }
}