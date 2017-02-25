using System.Web.Mvc;

namespace Bundler.Example.Application {
    public class ExampleWebViewPage<T> : WebViewPage<T> {

        public BundleManager Bundles { get; private set; }

        public override void Execute() {
           
        }
        
        public override void InitHelpers() {
            base.InitHelpers();
            var app = (MvcApplication) ViewContext.RequestContext.HttpContext.ApplicationInstance;
            Bundles = new BundleManager(app.BundleProvider, ViewContext.RequestContext);
        }
    }
}