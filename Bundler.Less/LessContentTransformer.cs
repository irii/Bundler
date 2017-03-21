using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        bool IContentTransformer.Process(IBundleContext bundleContext, IFileContent fileContent) {
            if (string.IsNullOrWhiteSpace(fileContent.Content)) {
                fileContent.Content = string.Empty;
                return true;
            }

            var configuration = new DotlessConfiguration {
                MinifyOutput = bundleContext.Optimization,
                MapPathsToWeb = false,
                DisableParameters = false,
                RootPath = bundleContext.GetFullPath(Path.GetDirectoryName(fileContent.VirtualFile))
            };

            var lessEngine = new EngineFactory(configuration).GetEngine();
            lessEngine.CurrentDirectory = configuration.RootPath;

            try {
                fileContent.Content = lessEngine.TransformToCss(fileContent.Content, null) ?? string.Empty;
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}