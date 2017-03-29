using System;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public static class AspNetBundler {
        private static readonly object WriteLock = new object();
        private static IBundleProvider _currentBundleProvider;

        public static bool IsReady { get; private set; }

        /// <summary>
        /// Current BundleProvider
        /// </summary>
        public static IBundleProvider Current {
            get {
                if (!IsReady) {
                    throw new Exception("Not initialized. Set BundleProvider before use!");
                }
                return _currentBundleProvider;
            }
            set {
                lock (WriteLock) {
                    Interlocked.Exchange(ref _currentBundleProvider, value);
                    IsReady = true;
                }
            }
        }
    }
}
