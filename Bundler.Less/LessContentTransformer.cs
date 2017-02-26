using System;
using Bundler.Infrastructure;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundleContext bundleContext, IFileContent fileContent) {
            if (string.IsNullOrWhiteSpace(fileContent.Content)) {
                fileContent.Content = string.Empty;
                return true;
            }

            fileContent.Content = dotless.Core.Less.Parse(fileContent.Content, new DotlessConfiguration {
                MinifyOutput = bundleContext.Optimization,
                RootPath = "~/",
            }) ?? string.Empty;

            return !string.IsNullOrWhiteSpace(fileContent.Content);
        }
    }
}