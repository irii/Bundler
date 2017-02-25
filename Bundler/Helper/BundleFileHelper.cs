﻿using System;
using System.IO;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    public static class BundleFileHelper {
        public static IBundle AddFile(this IBundle bundle, string virtualFile) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));

            if (!virtualFile.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                throw new ArgumentException("Path should be virtual!");
            }

            var absoluteFilePath = HttpContext.Current.Server.MapPath(virtualFile);

            if (!File.Exists(absoluteFilePath)) {
                return bundle;
            }

            var fileContent = File.ReadAllText(absoluteFilePath);
            bundle.Include(virtualFile, fileContent);
            return bundle;
        }
    }
}
