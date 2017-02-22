using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerModule : IHttpModule {
        public void Init(HttpApplication context) {
            context.PostResolveRequestCache += Context_PostResolveRequestCache;
        }

        private static void Context_PostResolveRequestCache(object sender, System.EventArgs e) {
            var app = (HttpApplication) sender;

            IBundle bundle;
            if (Bundler.Current.Get(app.Request.Url.AbsolutePath, out bundle)) {
                app.Context.RemapHandler(new BundlerHandler(bundle));
            }
        }

        public void Dispose() {}
    }
}
