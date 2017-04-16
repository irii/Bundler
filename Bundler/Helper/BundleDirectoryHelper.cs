using System;
using Bundler.Infrastructure;
using Bundler.Sources;

namespace Bundler.Helper {
    public static class BundleDirectoryHelper {
        /// <summary>
        /// Adds all files from a directory to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualDirectoryPath"></param>
        /// <param name="regexSearchPattern"></param>
        /// <param name="includeChildren"></param>
        /// <param name="throwExceptionOnError"></param>
        /// <returns></returns>
        public static IBundle AddDirectory(this IBundle bundle, string virtualDirectoryPath, string regexSearchPattern, bool includeChildren, bool throwExceptionOnError = false) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualDirectoryPath == null) throw new ArgumentNullException(nameof(virtualDirectoryPath));
            if (regexSearchPattern == null) throw new ArgumentNullException(nameof(regexSearchPattern));

            var includeResult = bundle.Add(new DirectorySource(virtualDirectoryPath, regexSearchPattern, includeChildren));

            if (!includeResult && throwExceptionOnError) {
                throw new Exception($"Failed to add '{virtualDirectoryPath}' to the bundle.");
            }

            return bundle;
        }
    }
}
