using System;

namespace Bundler.Infrastructure {
    public interface IBundleContext {
        /// <summary>
        /// Optimization
        /// </summary>
        bool Optimization { get; }

        /// <summary>
        /// Server cache
        /// </summary>
        bool Cache { get; }

        /// <summary>
        /// Append ETag
        /// </summary>
        bool ETag { get; }

        /// <summary>
        /// Server cache duration
        /// </summary>
        TimeSpan CacheDuration { get; }

        /// <summary>
        /// Use input content if processing has failed
        /// </summary>
        bool FallbackOnError { get; }

        /// <summary>
        /// Combine all files to one.
        /// </summary>
        bool BundleFiles { get; }

        /// <summary>
        /// Returns the full path of a virtual file
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string GetFullPath(string virtualPath);
    }
}