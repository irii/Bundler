using System;

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
        /// Array of inlcude files in this bundle.
        /// </summary>
        string[] Files { get; }

        /// <summary>
        /// Returns a the content of the requested file.
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        string GetFileContent(string virtualFile);
    }
}