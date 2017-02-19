using Bundler.Css;
using Bundler.Helper;
using Bundler.JavaScript;

namespace Bundler.Example {
    public class BundleConfig {
        public const string ApplicationScriptsBundleKey = "AppScripts";
        public const string ApplicationStylesBundleKey = "AppStyles";

        public static Bundle ScriptBundle;
        public static Bundle StyleBundle;

        public static void RegisterBundles() {
            ScriptBundle = Bundler.RegisterContentBundler(ApplicationScriptsBundleKey, "~/Scripts", JavaScriptContentBundler.Instance);
            StyleBundle = Bundler.RegisterContentBundler(ApplicationStylesBundleKey, "~/Styles", CssContentBundler.Instance);

            // Scripts
            ScriptBundle.AddFile("~/Scripts/jquery-1.10.2.js");
            ScriptBundle.AddFile("~/Scripts/jquery.validate.js");
            ScriptBundle.AddFile("~/Scripts/modernizr-2.6.2.js");
            ScriptBundle.AddFile("~/Scripts/bootstrap.js");
            ScriptBundle.AddFile("~/Scripts/respond.js");

            // Styles
            StyleBundle.AddFile("~/Content/bootstrap.css");
            StyleBundle.AddFile("~/Content/site.css");
        }
    }
}