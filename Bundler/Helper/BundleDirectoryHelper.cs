using System;
using System.IO;
using System.Linq;
using Bundler.Internals;

namespace Bundler.Helper {
    public static class BundleDirectoryHelper {
        public static Bundle AddDirectory(this Bundle bundle, string virtualPath, string searchPattern) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (searchPattern == null) throw new ArgumentNullException(nameof(searchPattern));

            if (!virtualPath.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            var absolutePath = VirtualPathFileHelper.GetFullPath(virtualPath);
            if (!Directory.Exists(absolutePath)) {
                return bundle;
            }

            var serverPath = VirtualPathFileHelper.GetFullPath("~/");
            var files = Directory.EnumerateFiles(absolutePath, searchPattern);
            foreach (var file in files.Where(x => x.StartsWith(serverPath, StringComparison.InvariantCultureIgnoreCase))) {
                var virtualFile = file.Replace(serverPath, "~/").Replace("\\", "/");

                var fileContent = File.ReadAllText(file);
                bundle.Add(virtualFile, fileContent);
            }

            return bundle;
        }
    }
}
