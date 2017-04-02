using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleResponse : IBundleResponse {
        public BundleResponse(string contentType, string contentHash, DateTime lastModification, string content, IReadOnlyDictionary<string, IBundleContent> files, IReadOnlyDictionary<string, string> headers) {
            ContentType = contentType;
            ContentHash = contentHash;
            LastModification = lastModification;
            Content = content;
            Files = files;
            Headers = headers;
        }

        public string ContentType { get; }
        public DateTime LastModification { get; }
        public string Content { get; }
        public IReadOnlyDictionary<string, IBundleContent> Files { get; }
        public string ContentHash { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}