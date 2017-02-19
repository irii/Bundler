using System;
using System.IO;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        public static Bundle AddFile(this Bundle bundle, string virtualFile) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));

            if (!VirtualPathFileHelper.Exists(virtualFile)) {
                return bundle;
            }

            var fileContent = File.ReadAllText(VirtualPathFileHelper.GetFilePath(virtualFile));
            bundle.Add(virtualFile, fileContent);
            return bundle;
        }
    }
}
