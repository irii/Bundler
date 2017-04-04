﻿using System.Runtime.CompilerServices;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public sealed class AspNetBundlerModule : IHttpModule {
        private readonly IBundleProvider _bundleProvider;
        public AspNetBundlerModule() {}

        public AspNetBundlerModule(IBundleProvider bundleProvider) {
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
