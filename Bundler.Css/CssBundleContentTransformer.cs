using System;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Transform;
using Microsoft.Ajax.Utilities;

namespace Bundler.Css {
    public class CssBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

        bool IBundleContentTransformer.Process(IBundle bundle, BundleTransformItem bundleTransformItemResult) {
            if (!bundle.Context.Configuration.Get(CssConfiguration.Minify) || string.IsNullOrWhiteSpace(bundleTransformItemResult.Content)) {
                bundleTransformItemResult.Content = bundleTransformItemResult.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            bundleTransformItemResult.Content = minifier.MinifyStyleSheet(bundleTransformItemResult.Content, new Microsoft.Ajax.Utilities.CssSettings { CommentMode = CssComment.None })?.Trim() ?? string.Empty;
            foreach (var contextError in minifier.ErrorList) {
                bundleTransformItemResult.Errors.Add(contextError.Message);
            }

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(bundleTransformItemResult.Content);
        }
    }
}