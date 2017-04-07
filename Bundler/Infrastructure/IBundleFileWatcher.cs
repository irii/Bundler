using System;

namespace Bundler.Infrastructure {
    public interface IBundleFileWatcher : IDisposable {
        /// <summary>
        /// Adds a file to the file system watcher.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void Watch(string virtualPath, FileChangedDelegate callback);

        /// <summary>
        /// Removes a registered callback to the given file.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void Unwatch(string virtualPath, FileChangedDelegate callback);

        /// <summary>
        /// Simulates a change event.
        /// Note: Virtual should be registerd.
        /// </summary>
        /// <param name="virtualPath"></param>
        void InvokeChange(string virtualPath);
    }
}