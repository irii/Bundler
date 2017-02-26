using System;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleFile : IBundleFile {
        public BundleFile(string virtualFile, int version, string content, DateTime lastModification) {
            VirtualFile = virtualFile;
            Version = version;
            Content = content;
            LastModification = lastModification;
        }

        public string VirtualFile { get; }
        public int Version { get; }
        public string Content { get; }
        public DateTime LastModification { get; }
    }
}