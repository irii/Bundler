using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleResponse : IBundleResponse {
        public BundleResponse(string contentType, string contentHash, DateTime lastModification, string content, IReadOnlyDictionary<string, IBundleContent> files) {
            ContentType = contentType;
            ContentHash = contentHash;
            LastModification = lastModification;
            Content = content;
            Files = files;
        }

        public string ContentType { get; }
        public DateTime LastModification { get; }
        public string Content { get; }
        public IReadOnlyDictionary<string, IBundleContent> Files { get; }
        public string ContentHash { get; }
    }
}