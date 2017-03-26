using System;
using System.IO;
using Bundler.Infrastructure;

namespace Bundler.ContentSources {
    public class StreamSource : IContentSource {
        private readonly IBundleVirtualPathProvider _virtualPathProvider;
        private readonly bool _fileListener;

        public StreamSource(IBundleVirtualPathProvider virtualPathProvider, string virtualFile, bool fileListener = true) {
            _virtualPathProvider = virtualPathProvider;
            _fileListener = fileListener;
            VirtualFile = virtualFile;

            if (fileListener) {
                virtualPathProvider.Watch(virtualFile, Watcher_Changed);
            }
        }

        private void Watcher_Changed(string virtualPath) => OnSourceChanged?.Invoke(this, new EventArgs());

        public string VirtualFile { get; }

        public string Get() {
            try {
                using (var stream = _virtualPathProvider.Open(VirtualFile)) {
                    using (var streamReader = new StreamReader(stream)) {
                        return streamReader.ReadToEnd();
                    }
                }
            } catch (Exception) {
                return null;
            }
        }

        public event SourceChangedEvent OnSourceChanged;

        public void Dispose() {
            if (_fileListener) {
                _virtualPathProvider.Unwatch(VirtualFile, Watcher_Changed);
            }
        }
    }
}