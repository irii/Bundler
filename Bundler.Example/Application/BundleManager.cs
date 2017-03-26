using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Helper;
using Bundler.Infrastructure;
using Bundler.Less;

namespace Bundler.Example.Application {
    public class BundleManager {
        private readonly IBundleProvider _bundleProvider;
        private readonly RequestContext _requestContext;

        public BundleManager(IBundleProvider bundleProvider, RequestContext requestContext) {
            _bundleProvider = bundleProvider;
            _requestContext = requestContext;
        }

        public IHtmlString Render(string virtualPath) => new MvcHtmlString(_bundleProvider.Render(virtualPath));

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

        private IBundle ResolveBundle(string virtualPath) {
            IBundle bundle;
            if (!_bundleProvider.Get(virtualPath, out bundle)) {
                _bundleProvider.Add(virtualPath, _bundleProvider.CreateLessBundle());

                if (!_bundleProvider.Get(virtualPath, out bundle)) {
                    throw new Exception($"Failed to create bundle. ({virtualPath})");
                }
            }

            return bundle;
        }

        public IHtmlString RenderDynamicScripts() {
            var key = ResolveDynamicVirtualPath(_requestContext.RouteData, "Scripts");
            return key == null
                ? MvcHtmlString.Empty
                : new MvcHtmlString(_bundleProvider.Render(key));
        }

        public IHtmlString RenderDynamicStyles() {
            var key = ResolveDynamicVirtualPath(_requestContext.RouteData, "Styles");
            return key == null
                ? MvcHtmlString.Empty
                : new MvcHtmlString(_bundleProvider.Render(key));
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