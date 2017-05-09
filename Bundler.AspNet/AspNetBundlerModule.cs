using System.Runtime.CompilerServices;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public sealed class AspNetBundlerModule : IHttpModule {
        public void Init(HttpApplication context) {
            context.PostResolveRequestCache += Context_PostResolveRequestCache;
        }

        private static void Context_PostResolveRequestCache(object sender, System.EventArgs e) {
            if (!AspNetBundler.IsInitialized) {
                return;
            }

            var app = (HttpApplication)sender;
            TryRemapHandler(app, AspNetBundler.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryRemapHandler(HttpApplication app, IBundleProvider provider) {
            IBundleContentResponse bundleResponse;
            string requestVersion;

            if (provider.GetResponse(app.Request.Url, out bundleResponse, out requestVersion)) {
                app.Context.RemapHandler(new AspNetBundlerHandler(provider.Context, bundleResponse, requestVersion));
            }
        }

        public void Dispose() {}
    }
}
