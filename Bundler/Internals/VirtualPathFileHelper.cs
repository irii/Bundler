using System.IO;
using System.Web;

namespace Bundler.Internals {
    public static class VirtualPathFileHelper {
        public static bool Exists(string virtualFile) {
            if (string.IsNullOrWhiteSpace(virtualFile)) {
                return false;
            }

            return File.Exists(HttpContext.Current.Server.MapPath(virtualFile));
        }

        public static string GetFilePath(string virtualFile) {
            return HttpContext.Current.Server.MapPath(virtualFile);
        }
    }
}
