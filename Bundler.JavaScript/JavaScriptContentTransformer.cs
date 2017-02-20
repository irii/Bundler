using System;
using Bundler.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace Bundler.JavaScript {
    public class JavaScriptContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(string inputContent, out string outputContent) {
            if (string.IsNullOrWhiteSpace(inputContent)) {
                outputContent = string.Empty;
                return true;
            }

            var minifier = new Minifier();
            outputContent = minifier.MinifyJavaScript(inputContent, new CodeSettings() { EvalTreatment = EvalTreatment.MakeImmediateSafe, PreserveImportantComments = false })?.Trim() ?? string.Empty;
            return true;
        }
    }
}