using System;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler {
    public static class Bundler {
        private static readonly object WriteLock = new object();
        private static IBundleProvider _currentBundleProvider;

        private static bool _isReady = false;
        public static bool IsReady => _isReady;

        /// <summary>
        /// Current BundleProvider
        /// </summary>
        public static IBundleProvider Current {
            get {
                if (!_isReady) {
                    throw new Exception("Not initialized. Set BundleProvider before use!");
                }
                return _currentBundleProvider;
            }
            set {
                lock (WriteLock) {
                    Interlocked.Exchange(ref _currentBundleProvider, value);
                    _isReady = true;
                }
            }
        }
    }
}
