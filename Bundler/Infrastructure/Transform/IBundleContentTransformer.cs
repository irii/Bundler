using System;

namespace Bundler.Infrastructure.Transform {
    public interface IBundleContentTransformer : IDisposable {
        bool Process(IBundle bundle, BundleTransformItem transformItem);
    }
}