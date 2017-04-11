using System;
using Bundler.Helper;
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
            ScriptBundle = bundleProvider.CreateScriptBundle(async: false);
            StyleBundle = bundleProvider.CreateLessBundle();

            if (!bundleProvider.Add(ApplicationScriptsBundleKey, ScriptBundle)) {
                throw new Exception("Can't setup script bundle.");
            }

            if (!bundleProvider.Add(ApplicationStylesBundleKey, StyleBundle)) {
                throw new Exception("Can't setup style bundle.");
            }

            // Scripts
            ScriptBundle.AddFile("~/Scripts/jquery-1.10.2.js", true)
                .AddFile("~/Scripts/jquery.validate.js", true)
                .AddFile("~/Scripts/modernizr-2.6.2.js", true)
                .AddFile("~/Scripts/bootstrap.js", true)
                .AddFile("~/Scripts/respond.js", true);

            // Styles
            StyleBundle.AddFile("~/Content/bootstrap.css", true)
                .AddFile("~/Content/site.css", true);
        }
    }
}