using System;
using Bundler.Infrastructure;
using Bundler.Sources;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        /// <summary>
        /// Adds a file to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        public static IBundle AddFile(this IBundle bundle, string virtualFile) {
            return AddFiles(bundle, new[] {virtualFile});
        }

        /// <summary>
        /// Adds multiple files to the bundle.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="virtualFiles"></param>
        /// <returns></returns>
        public static IBundle AddFiles(this IBundle bundle, string[] virtualFiles) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFiles == null) throw new ArgumentNullException(nameof(virtualFiles));

            var fileContent = new StreamSource(virtualFiles);
            var includeResult = bundle.Add(fileContent);

            if (!includeResult) {
                throw new Exception($"Failed to add '{string.Join("; ", virtualFiles)}' to the bundle.");
            }

            return bundle;
        }
    }
}
