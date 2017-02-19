using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerModule : IHttpModule {
        public void Init(HttpApplication context) {
            context.PostResolveRequestCache += Context_PostResolveRequestCache;
        }

        private static void Context_PostResolveRequestCache(object sender, System.EventArgs e) {
            var app = (HttpApplication) sender;

            BundleInfo bundleInfo;
            if (Bundler.IsBundleRequest(app.Request.Url, out bundleInfo)) {
                app.Context.RemapHandler(new BundlerHandler(bundleInfo));
            }
        }

        public void Dispose() {}
    }
}
