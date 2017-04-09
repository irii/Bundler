using System;
using System.IO;
using Bundler.Infrastructure;

namespace Bundler.Sources {
    public class StreamSource : ISource {
        private readonly IBundleContext _bundleContext;

        public bool IsWatchable { get; } = true;

        public StreamSource(IBundleContext bundleContext, string virtualFile) {
            _bundleContext = bundleContext;
            VirtualFile = virtualFile;
        }

        public string VirtualFile { get; }

        public string Get() {
            try {
                using (var stream = _bundleContext.VirtualPathProvider.Open(VirtualFile)) {
                    using (var streamReader = new StreamReader(stream)) {
                        return streamReader.ReadToEnd();
                    }
                }
            } catch (Exception) {
                return null;
            }
        }

        public void Dispose() {}
    }
}