using System;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleFile : IBundleContent {
        public BundleFile(string virtualFile, string contentHash, string content, DateTime lastModification) {
            VirtualFile = virtualFile;
            ContentHash = contentHash;
            Content = content;
            LastModification = lastModification;
        }

        public string VirtualFile { get; }
        public string ContentHash { get; }
        public string Content { get; }
        public DateTime LastModification { get; }
    }
}