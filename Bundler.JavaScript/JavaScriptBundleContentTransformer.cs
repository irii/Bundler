using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.JavaScript {
    public class JavaScriptBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

        bool IBundleContentTransformer.Process(IBundle bundle, IBundleContentTransformResult bundleContentTransformResult) {
            if (!bundle.Context.Configuration.Optimization || string.IsNullOrWhiteSpace(bundleContentTransformResult.Content)) {
                bundleContentTransformResult.Content = bundleContentTransformResult.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            bundleContentTransformResult.Content = minifier.MinifyJavaScript(bundleContentTransformResult.Content, new CodeSettings() {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            })?.Trim() ?? string.Empty;

            foreach (var contextError in minifier.ErrorList) {
                bundleContentTransformResult.Errors.Add(contextError.Message);
            }

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(bundleContentTransformResult.Content);
        }
    }
}