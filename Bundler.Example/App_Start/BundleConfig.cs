using Bundler.Css;
using Bundler.JavaScript;

namespace Bundler.Example {
    public class BundleConfig {
        public const string ApplicationScriptsBundleKey = "AppScripts";
        public const string ApplicationStylesBundleKey = "AppStyles";

        public static void RegisterBundles() {
            Bundler.RegisterContentBundler(ApplicationScriptsBundleKey, JavaScriptContentBundler.Instance);
            Bundler.RegisterContentBundler(ApplicationStylesBundleKey, CssContentBundler.Instance);

            // Scripts
            Bundler.RegisterFile(ApplicationScriptsBundleKey, "~/Scripts/jquery-1.10.2.js");
            Bundler.RegisterFile(ApplicationScriptsBundleKey, "~/Scripts/jquery.validate.js");
            Bundler.RegisterFile(ApplicationScriptsBundleKey, "~/Scripts/modernizr-2.6.2.js");
            Bundler.RegisterFile(ApplicationScriptsBundleKey, "~/Scripts/bootstrap.js");
            Bundler.RegisterFile(ApplicationScriptsBundleKey, "~/Scripts/respond.js");

            // Styles
            Bundler.RegisterFile(ApplicationStylesBundleKey, "~/Content/bootstrap.css");
            Bundler.RegisterFile(ApplicationStylesBundleKey, "~/Content/site.css");
        }
    }
}