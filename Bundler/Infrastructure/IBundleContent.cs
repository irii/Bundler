using System;

namespace Bundler.Infrastructure {
    public interface IBundleContent {
        /// <summary>
        /// Hash
        /// </summary>
        string ContentHash { get; }

        /// <summary>
        /// Transformed content
        /// </summary>
        string Content { get; }

        /// <summary>
        /// LastModification
        /// </summary>
        DateTime LastModification { get; }
    }
}