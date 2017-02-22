using System;

namespace Bundler.Infrastructure {
    public interface IBundleProviderConfiguration {
        bool Optimization { get; }
        bool Cache { get; }
        TimeSpan CacheDuration { get; }

        bool PassthroughOnError { get; }
    }
}