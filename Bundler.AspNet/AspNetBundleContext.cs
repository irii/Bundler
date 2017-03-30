using System;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleContext : IBundleContext {
        public AspNetBundleContext() : this(new AspNetBundleVirtualPathProvider()) { }

        public AspNetBundleContext(IBundleVirtualPathProvider bundleVirtualPathProvider)
            : this(bundleVirtualPathProvider, new DefaultBundleFileWatcher(bundleVirtualPathProvider)) { }

        public AspNetBundleContext(IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher)
            : this(bundleVirtualPathProvider, bundleFileWatcher, new AspNetBundleUrlHelper()) { }

        public AspNetBundleContext(IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher, IBundleUrlHelper bundleUrlHelper) {
            if (bundleVirtualPathProvider == null) throw new ArgumentNullException(nameof(bundleVirtualPathProvider));
            if (bundleFileWatcher == null) throw new ArgumentNullException(nameof(bundleFileWatcher));
            if (bundleUrlHelper == null) throw new ArgumentNullException(nameof(bundleUrlHelper));

            VirtualPathProvider = bundleVirtualPathProvider;
            Watcher = bundleFileWatcher;
            UrlHelper = bundleUrlHelper;
        }

        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool FallbackOnError { get; set; }
        public bool BundleFiles { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public bool ETag { get; set; }
        public bool AutoRefresh { get; set; }

        public IBundleVirtualPathProvider VirtualPathProvider { get; }
        public IBundleFileWatcher Watcher { get; }
        public IBundleUrlHelper UrlHelper { get; }

        public void Dispose() {
            VirtualPathProvider?.Dispose();
        }

    }
}
