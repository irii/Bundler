using System;

namespace Bundler.Infrastructure {
    public interface ISource : IDisposable {
        /// <summary>
        /// Is Source watchable by BundleFileWatcher
        /// </summary>
        bool IsWatchable { get; }

        /// <summary>
        /// Virtual contentSource
        /// </summary>
        string VirtualFile { get; }

        /// <summary>
        /// Returns the actual contentSource content
        /// </summary>
        /// <returns></returns>
        string Get();
    }
}
