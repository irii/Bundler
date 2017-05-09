using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleResponse : IBundleContentResponse {
        /// <summary>
        /// List of all Files
        /// </summary>
        IReadOnlyDictionary<string, IBundleContentResponse> Files { get; }
    }
}