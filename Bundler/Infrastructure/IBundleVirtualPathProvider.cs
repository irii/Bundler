using System;
using System.Collections.Generic;
using System.IO;

namespace Bundler.Infrastructure {
    public delegate void FileChangedDelegate(string virtualPath);

    public interface IBundleVirtualPathProvider : IDisposable {
        /// <summary>
        /// Open file based on the virtual path.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        Stream Open(string virtualPath);

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// Checks if the directory exists.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// Returns a list of all files in the virtual directory.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IEnumerable<string> EnumerateFiles(string virtualPath);

        /// <summary>
        /// Returns a list of all folder in the virtual directory.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IEnumerable<string> EnumerateDirectories(string virtualPath);

        /// <summary>
        /// Adds a folder or file to the file system watcher.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        bool Watch(string virtualPath, FileChangedDelegate callback);

        /// <summary>
        /// Removes a registered callback to the given folder or file.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        bool Unwatch(string virtualPath, FileChangedDelegate callback);

        /// <summary>
        /// Returns the full path.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string GetFullPath(string virtualPath);

        /// <summary>
        /// Returns the virtual path.
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        string GetVirtualPath(string absolutePath);
    }
}