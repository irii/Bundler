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

        /// <summary>
        /// Encodes the given input to match the url requirements.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Encode(string input);

        /// <summary>
        /// Decodes the encoded input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Decode(string input);
    }
}