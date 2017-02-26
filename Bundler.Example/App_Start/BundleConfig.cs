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
            ScriptBundle.AddFile("~/Scripts/jquery-1.10.2.js")
                .AddFile("~/Scripts/jquery.validate.js")
                .AddFile("~/Scripts/modernizr-2.6.2.js")
                .AddFile("~/Scripts/bootstrap.js")
                .AddFile("~/Scripts/respond.js");

            // Styles
            StyleBundle.AddFile("~/Content/bootstrap.css")
                .AddFile("~/Content/site.css");
        }
    }
}