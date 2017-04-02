using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleResponse : IBundleContentResponse {
        /// <summary>
        /// List of all Files
        /// </summary>
        IReadOnlyDictionary<string, IBundleContent> Files { get; }
    }

    public interface IBundleContentResponse : IBundleContent {
        /// <summary>
        /// ContentType
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Headers
        /// </summary>
        IReadOnlyDictionary<string, string> Headers { get; }

    }
}