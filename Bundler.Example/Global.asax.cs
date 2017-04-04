using System.Web.Mvc;
using System.Web.Routing;
using Bundler.AspNet;
using Bundler.Defaults;
using Bundler.Infrastructure;

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        public IBundleProvider BundleProvider => _bundleProviderInstance;
        private static IBundleProvider _bundleProviderInstance;

        private AspNetBundlerModule _aspNetBundlerModule;

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var bundleContext = new AspNetBundleContext(new DefaultBundleConfiguration {
                BundleFiles = true,
                Optimization = true,
                Cache = true,
                AutoRefresh = true,
                ETag = true
            });

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
