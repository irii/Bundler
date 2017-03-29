using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.JavaScript {
    public class JavaScriptContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundle bundle, IContentTransform contentTransform) {
            if (!bundle.Context.Optimization || string.IsNullOrWhiteSpace(contentTransform.Content)) {
                contentTransform.Content = contentTransform.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            contentTransform.Content = minifier.MinifyJavaScript(contentTransform.Content, new CodeSettings() {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            })?.Trim() ?? string.Empty;

            foreach (var contextError in minifier.ErrorList) {
                contentTransform.AddError(contextError.Message);
            }

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(contentTransform.Content);
        }
    }
}