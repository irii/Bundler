using System;
using System.IO;
using Bundler.Infrastructure;
using Bundler.Sources;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        /// <summary>
        /// Adds a file to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualFile"></param>
        /// <param name="throwExceptionOnError"></param>
        /// <returns></returns>
        public static IBundle AddFile(this IBundle bundle, string virtualFile, bool throwExceptionOnError = false) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));

            if (!virtualFile.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (!bundle.Context.VirtualPathProvider.FileExists(virtualFile)) {
                throw new FileNotFoundException("Can't find file", virtualFile);
            }
            
            var fileContent = new StreamSource(virtualFile);
            var includeResult = bundle.Add(fileContent);

            if (!includeResult && throwExceptionOnError) {
                throw new Exception($"Failed to add '{virtualFile}' to the bundle.");
            }

            return bundle;
        }
    }
}
