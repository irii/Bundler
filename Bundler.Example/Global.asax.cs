using System.Web.Mvc;
using System.Web.Routing;
using Bundler.AspNet;
using Bundler.Defaults;
using Bundler.Infrastructure;
using Bundler.JavaScript;
using Bundler.Less;

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        public IBundleProvider BundleProvider => _bundleProviderInstance;
        private static IBundleProvider _bundleProviderInstance;

        private AspNetBundlerModule _aspNetBundlerModule;

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var configuration = new DefaultBundleConfigurationBuilder()
                .SetupBundling(new BundlingSettings {
                    AutoRefresh = true,
                    CombineResponse = true,
                    FallbackOnError = true
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

            var bundleContext = new AspNetBundleContext(configuration, new DebugBundleDiagnostic());
            var bundleProvider = new BundleProvider(bundleContext);

            AspNetBundler.Current = bundleProvider;
            BundleConfig.SetupBundler(bundleProvider);

            _bundleProviderInstance = bundleProvider;
        }

        public override void Init() {
            base.Init();

            _aspNetBundlerModule = new AspNetBundlerModule(BundleProvider);
            _aspNetBundlerModule.Init(this);
        }

        public override void Dispose() {
            base.Dispose();
            _aspNetBundlerModule?.Dispose();
        }
    }
}
