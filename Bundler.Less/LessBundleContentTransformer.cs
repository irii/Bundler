using System;
using System.IO;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Transform;
using dotless.Core;
using dotless.Core.Importers;
using dotless.Core.Parser;
using dotless.Core.Stylizers;

namespace Bundler.Less {
    public class LessBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }
        
        protected virtual ILessEngine GetLessEngine(IBundle bundle, BundleTransformItem bundleTransformItemResult) {
            var parser = new Parser(new PlainStylizer(), new Importer(new DotLessVirtualFileReader(bundle.Context.VirtualPathProvider)));

            var compress = bundle.Context.Configuration.Get(LessConfiguration.Compress);
            var debug = bundle.Context.Configuration.Get(LessConfiguration.Debug);

            var lessEngine = new LessEngine(parser, new DotLessBundleLogger(bundle.Context.Diagnostic), compress, debug) {
                CurrentDirectory = Path.GetDirectoryName(bundleTransformItemResult.VirtualFile)
            };
            
            return lessEngine;
        }
        
        bool IBundleContentTransformer.Process(IBundle bundle, BundleTransformItem bundleTransformItemResult) {
            if (string.IsNullOrWhiteSpace(bundleTransformItemResult.Content)) {
                bundleTransformItemResult.Content = string.Empty;
                return true;
            }

            var lessEngine = GetLessEngine(bundle, bundleTransformItemResult);

            try {
                var transformedContent = lessEngine.TransformToCss(bundleTransformItemResult.Content, null) ?? string.Empty;
                if (!lessEngine.LastTransformationSuccessful) {
                    bundleTransformItemResult.Errors.Add($"Failed to process less/css file {bundleTransformItemResult.VirtualFile}");
                    bundleTransformItemResult.CanUseFallback = false;
                    return false;
                }

                bundleTransformItemResult.Content = transformedContent;

                // Register dependencies
                foreach (var import in lessEngine.GetImports()) {
                    bundleTransformItemResult.WatchPaths.Add(import);
                }

                return true;
            } catch (Exception ex) {
                bundleTransformItemResult.Errors.Add(ex.Message);
                bundleTransformItemResult.CanUseFallback = false;

                return false;
            }
        }


    }
}