using System;

namespace Bundler.Infrastructure {
    public interface IContentTransformer : IDisposable {
        bool Process(string inputContent, out string outputContent);
    }
}