using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleResponse {
        /// <summary>
        /// ContentType
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Unqiue version numer
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Last modification
        /// </summary>
        DateTime LastModification { get; }

        /// <summary>
        /// Response content
        /// </summary>
        string Content { get; }

        /// <summary>
        /// List of all Files
        /// </summary>
        IReadOnlyDictionary<string, IBundleFile> Files { get; }
    }
}