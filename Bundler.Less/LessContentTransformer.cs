using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessContentTransformer : IContentTransformer {
        void IDisposable.Dispose() { }

        protected virtual DotlessConfiguration GetDotlessConfiguration(IBundle bundle, IContentTransform contentTransform) {
            var configuration = new DotlessConfiguration {
                MinifyOutput = bundle.Context.Optimization,
                MapPathsToWeb = false,
                DisableParameters = false,
                CacheEnabled = false,
                RootPath = bundle.Context.VirtualPathProvider.GetPhysicalPath(Path.GetDirectoryName(contentTransform.VirtualPath))
            };

            return configuration;
        }

        bool IContentTransformer.Process(IBundle bundle, IContentTransform contentTransform) {
            if (string.IsNullOrWhiteSpace(contentTransform.Content)) {
                contentTransform.Content = string.Empty;
                return true;
            }

            var configuration = GetDotlessConfiguration(bundle, contentTransform);

            var lessEngine = new EngineFactory(configuration).GetEngine();
            lessEngine.CurrentDirectory = configuration.RootPath;

            try {
                contentTransform.Content = lessEngine.TransformToCss(contentTransform.Content, null) ?? string.Empty;

                // Register dependencies
                var imports = lessEngine.GetImports();
                foreach (var import in imports) {
                    bundle.Context.Watcher.Watch(bundle.Context.VirtualPathProvider.GetVirtualPath(import), bundle.ChangeHandler);
                }

                return true;
            } catch (Exception ex) {
                contentTransform.AddError(ex.Message);
                return false;
            }
        }
    }
}