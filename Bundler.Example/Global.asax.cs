using System.Web.Mvc;
using System.Web.Routing;

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        private readonly BundlerModule _bundlerModule = new BundlerModule();

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.SetupBundler(Bundler.Current);
        }

        public override void Init() {
            base.Init();
            _bundlerModule.Init(this);
        }
    }
}
