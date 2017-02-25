using System.Web.Mvc;
using System.Web.Routing;
using Bundler.Infrastructure;

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        public IBundleProvider BundleProvider => _bundleProviderInstance;
        private static IBundleProvider _bundleProviderInstance;

        private BundlerModule _bundlerModule;

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var bundleContext = new DefaultBundleContext(this);
            var bundleProvider = new BundleProvider(bundleContext);

            Bundler.Current = bundleProvider;
            BundleConfig.SetupBundler(bundleProvider);

            _bundleProviderInstance = bundleProvider;
        }

        public override void Init() {
            base.Init();

            _bundlerModule = new BundlerModule(BundleProvider);
            _bundlerModule.Init(this);
        }

        public override void Dispose() {
            base.Dispose();
            _bundlerModule?.Dispose();
        }
    }
}
