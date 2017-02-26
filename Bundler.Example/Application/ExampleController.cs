using System.Web.Mvc;
using System.Web.Routing;

namespace Bundler.Example.Application {
    public abstract class ExampleController : Controller {
        public BundleManager Bundles { get; private set; }

        protected override void Initialize(RequestContext requestContext) {
            base.Initialize(requestContext);
            var bundleProvider = ((MvcApplication) requestContext.HttpContext.ApplicationInstance).BundleProvider;
            Bundles = new BundleManager(bundleProvider, requestContext);
        }
    }
}