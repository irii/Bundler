using System;
using Bundler.Infrastructure.Configuration;

namespace Bundler.Infrastructure.Server {
    public interface IBundleContext : IDisposable {
        /// <summary>
        /// Virtual Path Provider
        /// </summary>
        IBundleVirtualPathProvider VirtualPathProvider { get; }

        /// <summary>
        /// File Watcher
        /// </summary>
        IBundleFileWatcher Watcher { get; }

        /// <summary>
        /// Url helper
        /// </summary>
        IBundleUrlHelper UrlHelper { get; }

        /// <summary>
        /// Configuration
        /// </summary>
        IBundleConfiguration Configuration { get; }

        /// <summary>
        /// Logging
        /// </summary>
        IBundleDiagnostic Diagnostic { get; }
    }
}