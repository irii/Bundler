using System;
using Bundler.Infrastructure;
using Bundler.JavaScript;
using Bundler.Less;

namespace Bundler.Example {
    public static class BundleConfig {
        public const string ApplicationScriptsBundleKey = "~/Scripts/Application";
        public const string ApplicationStylesBundleKey = "~/Styles/Application";

        public static IBundle ScriptBundle { get; private set; }
        public static IBundle StyleBundle { get; private set; }

        public static void SetupBundler(IBundleProvider bundleProvider) {
            var scriptBundleBuilder = bundleProvider.CreateScriptBundle(async: false);
            var styleBundleBuilder = bundleProvider.CreateLessBundle();

            // Scripts
            scriptBundleBuilder
                .AddFile("~/Scripts/jquery-1.10.2.js")
                .AddFile("~/Scripts/jquery.validate.js")
                .AddFile("~/Scripts/modernizr-2.6.2.js")
                .AddFile("~/Scripts/bootstrap.js")
                .AddFile("~/Scripts/respond.js");

            // Styles
            styleBundleBuilder
                .AddFile("~/Content/bootstrap.css")
                .AddFile("~/Content/site.css");

            // Create bundles
            ScriptBundle = scriptBundleBuilder.Create();
            StyleBundle = styleBundleBuilder.Create();

            // Add bundles to bundle provider
            if (!bundleProvider.Add(ApplicationScriptsBundleKey, ScriptBundle)) {
                throw new Exception("Can't setup script bundle.");
            }

            if (!bundleProvider.Add(ApplicationStylesBundleKey, StyleBundle)) {
                throw new Exception("Can't setup style bundle.");
            }
        }
    }
}