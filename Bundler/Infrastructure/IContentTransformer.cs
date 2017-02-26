using System;

namespace Bundler.Infrastructure {
    public interface IContentTransformer : IDisposable {
        bool Process(IBundleContext bundleContext, IFileContent fileContent);
    }
}