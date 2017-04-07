using System;
using Bundler.Defaults;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleContext : IBundleContext {
        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleDiagnostic diagnostic) : this(bundleConfiguration, diagnostic, new AspNetBundleVirtualPathProvider()) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleDiagnostic diagnostic, IBundleVirtualPathProvider bundleVirtualPathProvider)
            : this(bundleConfiguration, diagnostic, bundleVirtualPathProvider, new DefaultBundleFileWatcher(diagnostic, bundleVirtualPathProvider)) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleDiagnostic diagnostic, IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher)
            : this(bundleConfiguration, diagnostic, bundleVirtualPathProvider, bundleFileWatcher, new AspNetBundleUrlHelper()) { }

        public AspNetBundleContext(IBundleConfiguration bundleConfiguration, IBundleDiagnostic diagnostic, IBundleVirtualPathProvider bundleVirtualPathProvider, IBundleFileWatcher bundleFileWatcher, IBundleUrlHelper bundleUrlHelper) {
            if (bundleConfiguration == null) throw new ArgumentNullException(nameof(bundleConfiguration));
            if (diagnostic == null) throw new ArgumentNullException(nameof(diagnostic));
            if (bundleVirtualPathProvider == null) throw new ArgumentNullException(nameof(bundleVirtualPathProvider));
            if (bundleFileWatcher == null) throw new ArgumentNullException(nameof(bundleFileWatcher));
            if (bundleUrlHelper == null) throw new ArgumentNullException(nameof(bundleUrlHelper));

            Configuration = bundleConfiguration;
            Diagnostic = diagnostic;
            VirtualPathProvider = bundleVirtualPathProvider;
            Watcher = bundleFileWatcher;
            UrlHelper = bundleUrlHelper;
        }

        public IBundleConfiguration Configuration { get; }
        public IBundleVirtualPathProvider VirtualPathProvider { get; }
        public IBundleFileWatcher Watcher { get; }
        public IBundleUrlHelper UrlHelper { get; }

        public IBundleDiagnostic Diagnostic { get; }

        public void Dispose() {
            VirtualPathProvider?.Dispose();
        }
    }
}
