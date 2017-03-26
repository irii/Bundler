using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.JavaScript {
    public class JavaScriptContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundle bundle, IFileContent fileContent) {
            if (!bundle.Context.Optimization || string.IsNullOrWhiteSpace(fileContent.Content)) {
                fileContent.Content = fileContent.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            fileContent.Content = minifier.MinifyJavaScript(fileContent.Content, new CodeSettings() {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            })?.Trim() ?? string.Empty;

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(fileContent.Content);
        }
    }
}