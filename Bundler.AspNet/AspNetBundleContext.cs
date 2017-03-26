using System;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleContext : IBundleContext {
        public AspNetBundleContext() : this(new AspNetBundleVirtualPathProvider()) {}
        public AspNetBundleContext(IBundleVirtualPathProvider bundleVirtualPathProvider) {
            if (bundleVirtualPathProvider == null) throw new ArgumentNullException(nameof(bundleVirtualPathProvider));
            VirtualPathProvider = bundleVirtualPathProvider;
        }
        
        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool FallbackOnError { get; set; }
        public bool BundleFiles { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public bool ETag { get; set; }
        public bool AutoRefresh { get; set; }

        public IBundleVirtualPathProvider VirtualPathProvider { get; }

        public void Dispose() {
            VirtualPathProvider?.Dispose();
        }
    }
}
