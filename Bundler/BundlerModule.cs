using System.Runtime.CompilerServices;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerModule : IHttpModule {
        private readonly IBundleProvider _bundleProvider;
        public BundlerModule() {}

        public BundlerModule(IBundleProvider bundleProvider) {
            _bundleProvider = bundleProvider;
        }

        public void Init(HttpApplication context) {
            if (_bundleProvider == null) {
                context.PostResolveRequestCache += Context_PostResolveRequestCache_Default;
            } else {
                context.PostResolveRequestCache += Context_PostResolveRequestCache;
            }
        }

        private void Context_PostResolveRequestCache(object sender, System.EventArgs e) {
            var app = (HttpApplication)sender;
            TryRemapHandler(app, _bundleProvider);
        }

        private static void Context_PostResolveRequestCache_Default(object sender, System.EventArgs e) {
            var app = (HttpApplication) sender;
            TryRemapHandler(app, Bundler.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryRemapHandler(HttpApplication app, IBundleProvider provider) {
            int requestVersion;
            string requestFile;

            IBundle bundle;
            if (provider.ResolveUri(app.Request.Url, out bundle, out requestVersion, out requestFile)) {
                app.Context.RemapHandler(new BundlerHandler(bundle, requestVersion, requestFile));
            }
        }

        public void Dispose() {}
    }
}
