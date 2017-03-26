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

        public ActionResult Refresh() {
            foreach (var bundle in Bundler.Current.GetBundles()) {
                bundle.Refresh();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}