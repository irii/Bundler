using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Css;
using Bundler.JavaScript;

namespace Bundler.Example {
    public static class BundleManager {
        private static string ResolveDynamicKey(RouteData routeData, string additionalIdentifier) {
            if (!routeData.Values.ContainsKey("controller") || !routeData.Values.ContainsKey("action")) {
                return null;
            }

            var controller = routeData.Values["controller"] as string;
            var action = routeData.Values["action"] as string;

            if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action)) {
                return null;
            }

            return $"C:{controller}::A:{action}::I:{additionalIdentifier}";
        }

        public static IHtmlString RenderDynamicScripts(RequestContext requestContext) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Scripts");
            return key == null 
                ? MvcHtmlString.Empty 
                : Bundler.RenderTag(key);
        }

        public static IHtmlString RenderDynamicStyles(RequestContext requestContext) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Styles");
            return key == null
                ? MvcHtmlString.Empty
                : Bundler.RenderTag(key);
        }
        
        public static void AddScriptFile(RequestContext requestContext, string scriptFile) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Scripts");
            if(key == null) { return; }
            if (!Bundler.IsBundleKeyRegistered(key)) {
                Bundler.RegisterContentBundler(key, JavaScriptContentBundler.Instance);
            }
            Bundler.RegisterFile(key, scriptFile);
        }

        public static void AddCssFile(RequestContext requestContext, string cssFile) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Styles");
            if (key == null) { return; }
            if (!Bundler.IsBundleKeyRegistered(key)) {
                Bundler.RegisterContentBundler(key, CssContentBundler.Instance);
            }
            Bundler.RegisterFile(key, cssFile);
        }
    }
}