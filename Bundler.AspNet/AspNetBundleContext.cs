using System;
using Bundler.Defaults;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleContext : IBundleContext {
        public AspNetBundleContext(IBundleConfiguration bundleConfiguration) : this(bundleConfiguration, new AspNetBundleVirtualPathProvider()) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleVirtualPathProvider bundleVirtualPathProvider)
            : this(bundleConfiguration, bundleVirtualPathProvider, new DefaultBundleFileWatcher(bundleVirtualPathProvider)) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher)
            : this(bundleConfiguration, bundleVirtualPathProvider, bundleFileWatcher, new AspNetBundleUrlHelper()) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher, IBundleUrlHelper bundleUrlHelper) {
            if (bundleConfiguration == null) throw new ArgumentNullException(nameof(bundleConfiguration));
            if (bundleVirtualPathProvider == null) throw new ArgumentNullException(nameof(bundleVirtualPathProvider));
            if (bundleFileWatcher == null) throw new ArgumentNullException(nameof(bundleFileWatcher));
            if (bundleUrlHelper == null) throw new ArgumentNullException(nameof(bundleUrlHelper));

            Configuration = bundleConfiguration;
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

        public IBundleConfiguration Configuration { get; }
        public IBundleVirtualPathProvider VirtualPathProvider { get; }
        public IBundleFileWatcher Watcher { get; }
        public IBundleUrlHelper UrlHelper { get; }

        public void Dispose() {
            VirtualPathProvider?.Dispose();
        }

    }
}
