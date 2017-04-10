using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.configuration;
using dotless.Core.Importers;
using dotless.Core.Input;
using dotless.Core.Loggers;
using dotless.Core.Parser;
using dotless.Core.Stylizers;
using LogLevel = dotless.Core.Loggers.LogLevel;

namespace Bundler.Less {
    public class LessBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

#if EXPERIMENTAL
        protected virtual ILessEngine GetLessEngine(IBundle bundle, BundleContentTransform bundleContentTransformResult) {
            var rootPath = bundle.Context.VirtualPathProvider.GetPhysicalPath(Path.GetDirectoryName(bundleContentTransformResult.VirtualPath));

            var parser = new Parser(new PlainStylizer(), new Importer(new FileReader()));

            // TODO: Implement logger brigde, file bridge, ...
            var lessEngine = new LessEngine(parser, new NullLogger(LogLevel.Info), false, false) {
                CurrentDirectory = rootPath,
                Compress = bundle.Context.Configuration.Optimization,
                Debug = !bundle.Context.Configuration.Optimization,
            };
            
            return lessEngine;
        }
#else
        protected virtual ILessEngine GetLessEngine(IBundle bundle, BundleContentTransform bundleContentTransformResult) {
            var configuration = new DotlessConfiguration {
                MinifyOutput = bundle.Context.Configuration.Optimization,
                MapPathsToWeb = false,
                DisableParameters = false,
                CacheEnabled = false,
                RootPath = bundle.Context.VirtualPathProvider.GetPhysicalPath(Path.GetDirectoryName(bundleContentTransformResult.VirtualPath))
            };

            var lessEngine = new EngineFactory(configuration).GetEngine();
            lessEngine.CurrentDirectory = configuration.RootPath;
            return lessEngine;
        }
#endif

        bool IBundleContentTransformer.Process(IBundle bundle, BundleContentTransform bundleContentTransformResult) {
            if (string.IsNullOrWhiteSpace(bundleContentTransformResult.Content)) {
                bundleContentTransformResult.Content = string.Empty;
                return true;
            }

            var lessEngine = GetLessEngine(bundle, bundleContentTransformResult);

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