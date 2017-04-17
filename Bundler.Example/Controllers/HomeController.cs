using System.Web.Mvc;
using Bundler.Example.Application;

namespace Bundler.Example.Controllers {
    public class HomeController : ExampleController {
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Refresh(string returnUrl = null) {
            foreach (var bundle in Bundles.Provider.GetBundles()) {
                bundle.Refresh();
            }

            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}