using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Css;
using Bundler.Helper;
using Bundler.Infrastructure;
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
                : Bundler.Render(key);
        }

        public static IHtmlString RenderDynamicStyles(RequestContext requestContext) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Styles");
            return key == null
                ? MvcHtmlString.Empty
                : Bundler.Render(key);
        }
        
        public static void AddScriptFile(RequestContext requestContext, string scriptFile) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Scripts");
            if(key == null) { return; }

            Bundle bundle;
            if (!Bundler.TryGetBundle(key, out bundle)) {
                var path = $"~/Scripts/{requestContext.RouteData.Values["controller"]}/{requestContext.RouteData.Values["action"]}";
                Bundler.RegisterContentBundler(key, path, JavaScriptContentBundler.Instance);
            }

            bundle.AddFile(scriptFile);
        }

        public static void AddCssFile(RequestContext requestContext, string cssFile) {
            var key = ResolveDynamicKey(requestContext.RouteData, "Styles");
            if (key == null) { return; }

            Bundle bundle;
            if (!Bundler.TryGetBundle(key, out bundle)) {
                var path = $"~/Styles/{requestContext.RouteData.Values["controller"]}/{requestContext.RouteData.Values["action"]}";
                Bundler.RegisterContentBundler(key, path, CssContentBundler.Instance);
            }

            bundle.AddFile(cssFile);
        }
    }
}