using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleResponse : IBundleContent {
        /// <summary>
        /// ContentType
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// List of all Files
        /// </summary>
        IReadOnlyDictionary<string, IBundleContent> Files { get; }
    }
}