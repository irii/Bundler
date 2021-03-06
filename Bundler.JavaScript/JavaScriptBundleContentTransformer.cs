﻿using System;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Transform;
using Microsoft.Ajax.Utilities;

namespace Bundler.JavaScript {
    public class JavaScriptBundleContentTransformer : IBundleContentTransformer {
        void IDisposable.Dispose() { }

        bool IBundleContentTransformer.Process(IBundle bundle, BundleTransformItem bundleContentTransformResult) {
            if (!bundle.Context.Configuration.Get(JavaScriptConfiguration.Minify) || string.IsNullOrWhiteSpace(bundleContentTransformResult.Content)) {
                bundleContentTransformResult.Content = bundleContentTransformResult.Content.Trim();
                return true;
            }

            var minifier = new Minifier();
            bundleContentTransformResult.Content = minifier.MinifyJavaScript(bundleContentTransformResult.Content, new CodeSettings() {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            })?.Trim() ?? string.Empty;

            foreach (var contextError in minifier.ErrorList) {
                bundleContentTransformResult.Errors.Add(contextError.Message);
            }

            return minifier.ErrorList.Count == 0 && !string.IsNullOrWhiteSpace(bundleContentTransformResult.Content);
        }
    }
}