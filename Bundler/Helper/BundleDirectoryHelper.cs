using System;
using System.IO;
using System.Linq;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    public static class BundleDirectoryHelper {
        /// <summary>
        /// Adds all files from a directory to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualDirectoryPath"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IBundle AddDirectory(this IBundle bundle, string virtualDirectoryPath, string searchPattern) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualDirectoryPath == null) throw new ArgumentNullException(nameof(virtualDirectoryPath));
            if (searchPattern == null) throw new ArgumentNullException(nameof(searchPattern));

            if (!virtualDirectoryPath.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            var absolutePath = bundle.Context.GetFullPath(virtualDirectoryPath);
            if (!Directory.Exists(absolutePath)) {
                return bundle;
            }

            var serverPath = bundle.Context.GetFullPath("~/");
            var files = Directory.EnumerateFiles(absolutePath, searchPattern);
            foreach (var file in files.Where(x => x.StartsWith(serverPath, StringComparison.InvariantCultureIgnoreCase))) {
                var virtualFile = file.Replace(serverPath, "~/").Replace("\\", "/");

                var fileContent = File.ReadAllText(file);
                bundle.Include(virtualFile, fileContent);
            }

            return bundle;
        }
    }
}
