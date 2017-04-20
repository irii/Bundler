using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleResponse : IBundleResponse {
        public BundleResponse(string contentType, string contentHash, DateTimeOffset lastModification, string content, IReadOnlyDictionary<string, IBundleContentResponse> files, IReadOnlyDictionary<string, string> headers) {
            ContentType = contentType;
            ContentHash = contentHash;
            LastModification = lastModification;
            Content = content;
            Files = files;
            Headers = headers;
        }

        public string ContentType { get; }
        public DateTimeOffset LastModification { get; }
        public string Content { get; }
        public IReadOnlyDictionary<string, IBundleContentResponse> Files { get; }
        public string ContentHash { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}