using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.Css {
    public class CssBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

        bool IBundleContentTransformer.Process(IBundle bundle, BundleContentTransform bundleContentTransformResult) {
            if (!bundle.Context.Configuration.Optimization || string.IsNullOrWhiteSpace(bundleContentTransformResult.Content)) {
                bundleContentTransformResult.Content = bundleContentTransformResult.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            bundleContentTransformResult.Content = minifier.MinifyStyleSheet(bundleContentTransformResult.Content, new CssSettings { CommentMode = CssComment.None })?.Trim() ?? string.Empty;
            foreach (var contextError in minifier.ErrorList) {
                bundleContentTransformResult.Errors.Add(contextError.Message);
            }

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(bundleContentTransformResult.Content);
        }
    }
}