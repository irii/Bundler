using System;

namespace Bundler.Infrastructure {
    public interface IBundle {
        string TagFormat { get; }

        /// <summary>
        /// Context
        /// </summary>
        IBundleContext Context { get; }

        /// <summary>
        /// Returns a response object for the current bundle.
        /// </summary>
        /// <returns></returns>
        IBundleResponse GetResponse();

        /// <summary>
        /// Includes a new file to the Bundle
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool Include(string virtualFile, string content);
    }

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
        string[] BundleFiles { get; }

        /// <summary>
        /// Returns a the content of the requested file.
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        string GetFile(string virtualFile);
    }
}