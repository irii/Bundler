using System;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public static class AspNetBundler {
        /// <summary>
        /// Bundler IIS IHttpModule
        /// </summary>
        public static Type HttpModule { get; } = typeof(AspNetBundlerModule);

        /// <summary>
        /// Bundler IIS IHttpHandler
        /// </summary>
        public static Type HttpHandler { get; } = typeof(AspNetBundlerHandler);

        private static readonly object WriteLock = new object();
        private static IBundleProvider _currentBundleProvider;

        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Current BundleProvider
        /// </summary>
        public static IBundleProvider Current {
            get {
                if (!IsInitialized) {
                    throw new Exception("Not initialized. Set BundleProvider before use!");
                }
                return _currentBundleProvider;
            }
        }

        /// <summary>
        /// Set BundleProvider for request handling
        /// </summary>
        /// <param name="bundleProvider"></param>
        public static void Initialize(IBundleProvider bundleProvider) {
            if (bundleProvider == null) throw new ArgumentNullException(nameof(bundleProvider));

            lock (WriteLock) {
                Interlocked.Exchange(ref _currentBundleProvider, bundleProvider);
                IsInitialized = true;
            }
        }
    }
}
