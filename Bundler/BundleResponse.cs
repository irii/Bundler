using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleResponse : IBundleResponse {
        public BundleResponse(string contentType, int version, DateTime lastModification, string content, IReadOnlyDictionary<string, IBundleFile> files) {
            ContentType = contentType;
            Version = version;
            LastModification = lastModification;
            Content = content;
            Files = files;
        }

        public string ContentType { get; }
        public int Version { get; }
        public DateTime LastModification { get; }
        public string Content { get; }
        public IReadOnlyDictionary<string, IBundleFile> Files { get; }
    }
}