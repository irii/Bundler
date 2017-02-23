using System.Web;

namespace Bundler.Internals {
    public static class VirtualPathHelper {
        public static string GetFullPath(string virtualFile) {
            return HttpContext.Current.Server.MapPath(virtualFile);
        }
    }
}
