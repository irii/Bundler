using System;
using Bundler.Infrastructure;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(string inputContent, out string outputContent) {
            if (string.IsNullOrWhiteSpace(inputContent)) {
                outputContent = string.Empty;
                return true;
            }

            outputContent = dotless.Core.Less.Parse(inputContent, new DotlessConfiguration {
                MinifyOutput = true
            }) ?? string.Empty;

            return true;
        }
    }
}