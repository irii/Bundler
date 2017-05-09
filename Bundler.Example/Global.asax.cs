﻿using System.Web.Mvc;
using System.Web.Routing;
using Bundler.AspNet;
using Bundler.Defaults;
using Bundler.Infrastructure;
using Bundler.JavaScript;
using Bundler.Less;

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        public IBundleProvider BundleProvider => AspNetBundler.Current;

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var configuration = new DefaultBundleConfigurationBuilder()
                .SetupBundling(new BundlingSettings {
                    AutoRefresh = true,
                    CombineResponse = true,
                    FallbackOnError = true,
                    IncludeContentHash = true
                })
                .SetupCaching(new CachingSettings {
                    Enabled = true,
                    UseEtag = true
                })
                .SetupLess(new LessSettings {
                    Compress = true,
                    Debug = true
                })
                .SetupJavaScript(new JavaScriptSettings {
                    Minify = true
                })
                .Create();

            // Setup BundleProvider
            var bundleContext = new AspNetBundleContext(configuration, new DebugBundleDiagnostic());
            var bundleProvider = new BundleProvider(bundleContext);

            // Init initial bundles
            BundleConfig.SetupBundler(bundleProvider);

            // Initialize Bundling engine
            AspNetBundler.Initialize(bundleProvider);
        }
    }
}
