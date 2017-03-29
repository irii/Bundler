using System;
using System.IO;
using Bundler.ContentSources;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        /// <summary>
        /// Adds a file to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        public static IBundle AddFile(this IBundle bundle, string virtualFile) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));

            if (!virtualFile.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (!bundle.Context.VirtualPathProvider.FileExists(virtualFile)) {
                throw new FileNotFoundException("Can't find file", virtualFile);
            }
            
            var fileContent = new StreamSource(bundle.Context, virtualFile);
            bundle.Include(fileContent);
            return bundle;
        }
    }
}
