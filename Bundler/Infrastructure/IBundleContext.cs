using System;

namespace Bundler.Infrastructure {
    public interface IBundleContext {
        bool Optimization { get; }
        bool Cache { get; }
        bool FallbackOnError { get; }
        TimeSpan CacheDuration { get; }

        string GetFullPath(string virtualPath);
    }
}