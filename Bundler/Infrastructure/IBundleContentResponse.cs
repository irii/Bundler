using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleContentResponse {
        /// <summary>
        /// ContentType
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Headers
        /// </summary>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        /// Hash
        /// </summary>
        string ContentHash { get; }

        /// <summary>
        /// Transformed content
        /// </summary>
        string Content { get; }

        /// <summary>
        /// LastModification (UTC)
        /// </summary>
        DateTimeOffset LastModification { get; }
    }
}