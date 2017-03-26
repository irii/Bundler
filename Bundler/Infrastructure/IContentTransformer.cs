using System;

namespace Bundler.Infrastructure {
    public interface IContentTransformer : IDisposable {
        bool Process(IBundle bundle, IFileContent fileContent);
    }
}