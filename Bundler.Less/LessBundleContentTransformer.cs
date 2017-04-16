using System;
using System.IO;
using Bundler.Infrastructure;
using dotless.Core;
using dotless.Core.Importers;
using dotless.Core.Parser;
using dotless.Core.Stylizers;

namespace Bundler.Less {
    public class LessBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }
        
        protected virtual ILessEngine GetLessEngine(IBundle bundle, BundleTransformItem bundleTransformItemResult) {
            var parser = new Parser(new PlainStylizer(), new Importer(new DotLessVirtualFileReader(bundle.Context.VirtualPathProvider)));
            
            var lessEngine = new LessEngine(parser, new DotLessBundleLogger(bundle.Context.Diagnostic), bundle.Context.Configuration.Optimization, !bundle.Context.Configuration.Optimization) {
                CurrentDirectory = Path.GetDirectoryName(bundleTransformItemResult.VirtualPath)
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
                    bundleTransformItemResult.Errors.Add($"Failed to process less/css file {bundleTransformItemResult.VirtualPath}");
                    return false;
                }

                bundleTransformItemResult.Content = transformedContent;

                // Register dependencies
                var imports = lessEngine.GetImports();
                foreach (var import in imports) {
                    bundle.Context.Watcher.Watch(import, bundle.ChangeHandler);
                }

                return true;
            } catch (Exception ex) {
                bundleTransformItemResult.Errors.Add(ex.Message);
                return false;
            }
        }


    }
}