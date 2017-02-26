using System.Threading;
using Bundler.Infrastructure;

namespace Bundler {
    public static class Bundler {
        private static readonly IBundleProvider DefaultBundleProvider = new BundleProvider(new DefaultBundleContext());

        private static readonly object WriteLock = new object();
        private static IBundleProvider _currentBundleProvider;

        /// <summary>
        /// Current BundleProvider
        /// </summary>
        public static IBundleProvider Current {
            get { return _currentBundleProvider ?? DefaultBundleProvider; }
            set {
                lock (WriteLock) {
                    Interlocked.Exchange(ref _currentBundleProvider, value);
                }
            }
        }
    }
}
