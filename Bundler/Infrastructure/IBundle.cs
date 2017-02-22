using System;

namespace Bundler.Infrastructure {
    public interface IBundle {
        string VirtualPath { get; }

        string ContentType { get; }
        
        string Content { get; }
        DateTime LastModification { get; }
        int Version { get; }

        bool Append(string identifier, string content);

        string GenerateTag(string url);
    }
}