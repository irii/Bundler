using System;

namespace Bundler.Infrastructure {
    public interface IContentTransformer : IDisposable {
        bool Process(IBundleContext bundleContext, string inputContent, out string outputContent);
    }
}