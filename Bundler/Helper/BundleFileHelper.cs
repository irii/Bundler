using System;
using System.IO;
using Bundler.Internals;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        public static Bundle AddFile(this Bundle bundle, string virtualFile) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));

            if (!virtualFile.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (!VirtualPathFileHelper.Exists(virtualFile)) {
                return bundle;
            }

            var fileContent = File.ReadAllText(VirtualPathFileHelper.GetFullPath(virtualFile));
            bundle.Add(virtualFile, fileContent);
            return bundle;
        }
    }
}
