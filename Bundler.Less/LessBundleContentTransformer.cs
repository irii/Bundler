using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.configuration;

namespace Bundler.Less {
    public class LessBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

        protected virtual DotlessConfiguration GetDotlessConfiguration(IBundle bundle, IBundleContentTransformResult bundleContentTransformResult) {
            var configuration = new DotlessConfiguration {
                MinifyOutput = bundle.Context.Configuration.Optimization,
                MapPathsToWeb = false,
                DisableParameters = false,
                CacheEnabled = false,
                RootPath = bundle.Context.VirtualPathProvider.GetPhysicalPath(Path.GetDirectoryName(bundleContentTransformResult.VirtualPath))
            };

            return configuration;
        }

        bool IBundleContentTransformer.Process(IBundle bundle, IBundleContentTransformResult bundleContentTransformResult) {
            if (string.IsNullOrWhiteSpace(bundleContentTransformResult.Content)) {
                bundleContentTransformResult.Content = string.Empty;
                return true;
            }

            var configuration = GetDotlessConfiguration(bundle, bundleContentTransformResult);

            var lessEngine = new EngineFactory(configuration).GetEngine();
            lessEngine.CurrentDirectory = configuration.RootPath;

            try {
                bundleContentTransformResult.Content = lessEngine.TransformToCss(bundleContentTransformResult.Content, null) ?? string.Empty;

                // Register dependencies
                var imports = lessEngine.GetImports();
                foreach (var import in imports) {
                    bundle.Context.Watcher.Watch(bundle.Context.VirtualPathProvider.GetVirtualPath(import), bundle.ChangeHandler);
                }

                return true;
            } catch (Exception ex) {
                bundleContentTransformResult.Errors.Add(ex.Message);
                return false;
            }
        }
    }
}