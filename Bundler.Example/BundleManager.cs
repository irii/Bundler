using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Css;
using Bundler.Helper;
using Bundler.Infrastructure;
using Bundler.JavaScript;

namespace Bundler.Example {
    public static class BundleManager {
        private static string ResolveDynamicVirtualPath(RouteData routeData, string additionalIdentifier) {
            if (!routeData.Values.ContainsKey("controller") || !routeData.Values.ContainsKey("action")) {
                return null;
            }

            var controller = routeData.Values["controller"] as string;
            var action = routeData.Values["action"] as string;

            if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action)) {
                return null;
            }

            return $"~/Dynamic/{controller}/{action}/{additionalIdentifier}";
        }

        public static IHtmlString RenderDynamicScripts(RequestContext requestContext) {
            var key = ResolveDynamicVirtualPath(requestContext.RouteData, "Scripts");
            return key == null
                ? MvcHtmlString.Empty
                : Bundler.Render(key);
        }

        public static IHtmlString RenderDynamicStyles(RequestContext requestContext) {
            var key = ResolveDynamicVirtualPath(requestContext.RouteData, "Styles");
            return key == null
                ? MvcHtmlString.Empty
                : Bundler.Render(key);
        }

        public static void AddScriptFile(RequestContext requestContext, string scriptFile) {
            var virtualPath = ResolveDynamicVirtualPath(requestContext.RouteData, "Scripts");
            if (virtualPath == null) { return; }

            var bundleProvider = Bundler.Current;

            IBundle bundle;
            if (!bundleProvider.Get(virtualPath, out bundle)) {
                bundleProvider.Add(virtualPath, bundleProvider.CreateScriptBundle());

                if (!bundleProvider.Get(virtualPath, out bundle)) {
                    throw new Exception($"Failed to create bundle. ({virtualPath})");
                }
            }
            
            bundle.AddFile(scriptFile);
        }

        public static void AddCssFile(RequestContext requestContext, string cssFile) {
            var virtualPath = ResolveDynamicVirtualPath(requestContext.RouteData, "Styles");
            if (virtualPath == null) { return; }

            var bundleProvider = Bundler.Current;

            IBundle bundle;
            if (!bundleProvider.Get(virtualPath, out bundle)) {
                bundleProvider.Add(virtualPath, bundleProvider.CreateCssBundle());

                if (!bundleProvider.Get(virtualPath, out bundle)) {
                    throw new Exception($"Failed to create bundle. ({virtualPath})");
                }
            }

            bundle.AddFile(cssFile);
        }
    }
}