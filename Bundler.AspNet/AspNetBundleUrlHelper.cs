using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleUrlHelper : IBundleUrlHelper {
        public string ToAbsolute(string virtualUrl) {
            return VirtualPathUtility.ToAbsolute(virtualUrl);
        }

        public IDictionary<string, string> ParseQuery(string query) {
            var nvc = HttpUtility.ParseQueryString(query ?? string.Empty);
            return nvc.AllKeys.Distinct().ToDictionary(x => x, y => nvc[y]);
        }
    }
}