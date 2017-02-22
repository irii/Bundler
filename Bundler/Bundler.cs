﻿using System.Threading;
using System.Web;
using System.Web.Mvc;
using Bundler.Infrastructure;

namespace Bundler {
    public static class Bundler {
        private static readonly IBundleProvider DefaultBundleProvider = new BundleProvider(null);

        private static readonly object WriteLock = new object();
        private static IBundleProvider _currentBundleProvider;

        public static IBundleProvider Current {
            get { return _currentBundleProvider ?? DefaultBundleProvider; }
            set {
                lock (WriteLock) {
                    Interlocked.Exchange(ref _currentBundleProvider, value);
                }
            }
        }

        public static IHtmlString Render(string virtualPath) {
            IBundle bundle;
            if (Current.Get(virtualPath, out bundle)) {
                return MvcHtmlString.Create(bundle.GenerateTag(VirtualPathUtility.ToAbsolute(virtualPath) + "?v=" + bundle.Version));
            }
            
            return MvcHtmlString.Empty;
        }
    }
}
