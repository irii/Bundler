using System.Web;

namespace Bundler.Internals {
    public static class VirtualPathFileHelper {
        public static string GetFullPath(string virtualFile) {
            return HttpContext.Current.Server.MapPath(virtualFile);
        }
    }
}
