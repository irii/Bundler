using System;

namespace Bundler.Infrastructure {
    public interface IBundleContext : IDisposable {
        /// <summary>
        /// Virtual Path Provider
        /// </summary>
        IBundleVirtualPathProvider VirtualPathProvider { get; }

        /// <summary>
        /// File Watcher
        /// </summary>
        IBundleFileWatcher Watcher { get; }

        /// <summary>
        /// Url helper
        /// </summary>
        IBundleUrlHelper UrlHelper { get; }

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
        /// Refresh bundle on source content change.
        /// </summary>
        bool AutoRefresh { get; }
    }
}