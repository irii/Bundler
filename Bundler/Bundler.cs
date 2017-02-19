using System;
using System.Web;
using System.Web.Mvc;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public static class Bundler {
        /// <summary>
        /// Check's if the request url matches to a bundle.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static bool IsBundleRequest(Uri uri, out Bundle bundle) {
            return BundleStore.GetBundleByPath(uri.AbsolutePath, out bundle);
        }

        public static Bundle AddBundle(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            if (bundleKey == null) throw new ArgumentNullException(nameof(bundleKey));
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (contentBundler == null) throw new ArgumentNullException(nameof(contentBundler));

            if (!virtualPath.StartsWith("~/")) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (virtualPath.Contains("?")) {
                throw new ArgumentException("Query arguments are not allowed!");
            }

            return BundleStore.RegisterKey(bundleKey, virtualPath.Substring(1), contentBundler);
        }

        public static bool TryGetBundle(string bundleKey, out Bundle bundle) {
            return BundleStore.GetBundleByKey(bundleKey, out bundle);
        }

        public static IHtmlString Render(string bundleKey) {
            Bundle bundle;
            if (!BundleStore.GetBundleByKey(bundleKey, out bundle)) {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(Environment.NewLine + bundle.Render());
        }
    }
}
