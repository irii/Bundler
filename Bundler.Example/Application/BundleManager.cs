using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Helper;
using Bundler.Infrastructure;
using Bundler.Less;

namespace Bundler.Example.Application {
    public class BundleManager {
        private readonly RequestContext _requestContext;

        public BundleManager(IBundleProvider provider, RequestContext requestContext) {
            Provider = provider;
            _requestContext = requestContext;
        }

        public IBundleProvider Provider { get; }

        public IBundle ApplicationStyles => BundleConfig.StyleBundle;
        public IBundle ApplicationScripts => BundleConfig.ScriptBundle;

        public IHtmlString Render(string virtualPath) => new MvcHtmlString(Provider.Render(virtualPath));

        private static string ResolveDynamicVirtualPath(RouteData routeData, string typeIdentifier) {
            if (!routeData.Values.ContainsKey("controller") || !routeData.Values.ContainsKey("action")) {
                return null;
            }

            var controller = routeData.Values["controller"] as string;
            var action = routeData.Values["action"] as string;

            if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action)) {
                return null;
            }

            return $"~/Dynamic/{controller}/{action}/{typeIdentifier}";
        }

        private IBundle ResolveBundle(string virtualPath) {
            IBundle bundle;
            if (!Provider.Get(virtualPath, out bundle)) {
                Provider.Add(virtualPath, Provider.CreateLessBundle());

                if (!Provider.Get(virtualPath, out bundle)) {
                    throw new Exception($"Failed to create bundle. ({virtualPath})");
                }
            }

            return bundle;
        }

        public IHtmlString RenderDynamicScripts() {
            var key = ResolveDynamicVirtualPath(_requestContext.RouteData, "Scripts");
            return key == null
                ? MvcHtmlString.Empty
                : new MvcHtmlString(Provider.Render(key));
        }

        public IHtmlString RenderDynamicStyles() {
            var key = ResolveDynamicVirtualPath(_requestContext.RouteData, "Styles");
            return key == null
                ? MvcHtmlString.Empty
                : new MvcHtmlString(Provider.Render(key));
        }

        public void AddScriptFile(string scriptFile) {
            var virtualPath = ResolveDynamicVirtualPath(_requestContext.RouteData, "Scripts");
            if (virtualPath == null) { return; }

            var bundle = ResolveBundle(virtualPath);
            bundle.AddFile(scriptFile);
        }

        public void AddCssFile(string cssFile) {
            var virtualPath = ResolveDynamicVirtualPath(_requestContext.RouteData, "Styles");
            if (virtualPath == null) { return; }

            var bundle = ResolveBundle(virtualPath);
            bundle.AddFile(cssFile);
        }
    }
}
