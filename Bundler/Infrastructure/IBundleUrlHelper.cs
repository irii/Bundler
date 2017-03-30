using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleUrlHelper {
        string ToAbsolute(string virtualUrl);

        /// <summary>
        /// Returns a case insensetive dictionary with all query parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IDictionary<string, string> ParseQuery(string query);
    }
}