using Bundler.Infrastructure;

namespace Bundler {
    public class BundleFile : IBundleFile {
        public BundleFile(string virtualFile, int version, string content) {
            VirtualFile = virtualFile;
            Version = version;
            Content = content;
        }

        public string VirtualFile { get; }
        public int Version { get; }
        public string Content { get; }
    }
}