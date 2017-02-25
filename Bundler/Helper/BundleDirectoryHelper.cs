using System;
using System.IO;
using System.Linq;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler.Helper {
    public static class BundleDirectoryHelper {
        public static IBundle AddDirectory(this IBundle bundle, string virtualPath, string searchPattern) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (searchPattern == null) throw new ArgumentNullException(nameof(searchPattern));

            if (!virtualPath.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            var absolutePath = VirtualPathHelper.GetFullPath(virtualPath);
            if (!Directory.Exists(absolutePath)) {
                return bundle;
            }

            var serverPath = VirtualPathHelper.GetFullPath("~/");
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
