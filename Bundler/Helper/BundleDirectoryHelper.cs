using System;
using System.IO;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    /// <summary>
    /// TODO: Add directory changed listener
    /// </summary>
    public static class BundleDirectoryHelper {
        /// <summary>
        /// Adds all files from a directory to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualDirectoryPath"></param>
        /// <param name="regexSearchPattern"></param>
        /// <param name="includeChildren"></param>
        /// <param name="enableFileChangeListener"></param>
        /// <returns></returns>
        public static IBundle AddDirectory(this IBundle bundle, string virtualDirectoryPath, string regexSearchPattern, bool includeChildren, bool enableFileChangeListener = true) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualDirectoryPath == null) throw new ArgumentNullException(nameof(virtualDirectoryPath));
            if (regexSearchPattern == null) throw new ArgumentNullException(nameof(regexSearchPattern));

            if (!virtualDirectoryPath.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (!bundle.Context.VirtualPathProvider.DirectoryExists(virtualDirectoryPath)) {
                throw new DirectoryNotFoundException($"Can't find directory '{virtualDirectoryPath}'.");
            }

            foreach (var file in bundle.Context.VirtualPathProvider.EnumerateFiles(virtualDirectoryPath)) {
                bundle.AddFile(file, enableFileChangeListener);

                if (includeChildren) {
                    foreach (var childDirectory in bundle.Context.VirtualPathProvider.EnumerateDirectories(virtualDirectoryPath)) {
                        AddDirectory(bundle, childDirectory, regexSearchPattern, true, enableFileChangeListener);

                    }
                }
            }

            return bundle;
        }
    }
}
