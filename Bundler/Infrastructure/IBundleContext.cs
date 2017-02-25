using System;

namespace Bundler.Infrastructure {
    public interface IBundleContext {
        bool Optimization { get; }
        bool Cache { get; }
        bool FallbackOnError { get; }
        TimeSpan CacheDuration { get; }

        bool BundleFiles { get; }

        string GetFullPath(string virtualPath);
    }
}