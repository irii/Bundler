using System;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    public class DefaultBundleConfiguration : IBundleConfiguration {
        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool ETag { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public bool FallbackOnError { get; set; }
        public bool BundleFiles { get; set; }
        public bool AutoRefresh { get; set; }
    }
}
