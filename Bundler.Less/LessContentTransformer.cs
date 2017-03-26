using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        protected virtual DotlessConfiguration GetDotlessConfiguration(IBundle bundle, IFileContent fileContent) {
            var configuration = new DotlessConfiguration {
                MinifyOutput = bundle.Context.Optimization,
                MapPathsToWeb = false,
                DisableParameters = false,
                RootPath = bundle.Context.VirtualPathProvider.GetFullPath(Path.GetDirectoryName(fileContent.VirtualFile))
            };

            return configuration;
        }

        bool IContentTransformer.Process(IBundle bundle, IFileContent fileContent) {
            if (string.IsNullOrWhiteSpace(fileContent.Content)) {
                fileContent.Content = string.Empty;
                return true;
            }

            var configuration = GetDotlessConfiguration(bundle, fileContent);

            var lessEngine = new EngineFactory(configuration).GetEngine();
            lessEngine.CurrentDirectory = configuration.RootPath;

            try {
                fileContent.Content = lessEngine.TransformToCss(fileContent.Content, null) ?? string.Empty;

                // Register dependencies
                var imports = lessEngine.GetImports();
                foreach (var import in imports) {
                    bundle.Context.VirtualPathProvider.Watch(bundle.Context.VirtualPathProvider.GetVirtualPath(import), bundle.ChangeHandler);
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}