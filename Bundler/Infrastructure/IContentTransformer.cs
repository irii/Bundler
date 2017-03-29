using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IContentTransformer : IDisposable {
        bool Process(IBundle bundle, IContentTransform contentTransform);
    }

    public interface IContentTransform {
        string VirtualPath { get; }
        string Content { get; set; }

        IReadOnlyCollection<string> Errors { get; }

        void AddError(string logMessage);
    }
}