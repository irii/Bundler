using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleFile : IBundleContentResponse {
        public BundleFile(string virtualFile, string contentType, string contentHash, string content, DateTimeOffset lastModification) {
            VirtualFile = virtualFile;
            ContentType = contentType;
            ContentHash = contentHash;
            Content = content;
            LastModification = lastModification;
        }

        public string ContentType { get; }
        public string VirtualFile { get; }
        public string ContentHash { get; }
        public string Content { get; }
        public DateTimeOffset LastModification { get; }

        public IReadOnlyDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    }
}