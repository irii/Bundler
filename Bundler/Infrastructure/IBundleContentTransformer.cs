using System;

namespace Bundler.Infrastructure {
    public interface IBundleContentTransformer : IDisposable {
        bool Process(IBundle bundle, BundleContentTransform bundleContentTransform);
    }
}