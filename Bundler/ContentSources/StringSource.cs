using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.ContentSources {
    public class StringSource : IContentSource {
        private readonly object _writeLock = new object();
        private string _content;

        public StringSource(string virtualFile, string content) {
            VirtualFile = virtualFile;
            Content = content;
        }

        public bool IsWatchable { get; } = false;

        public string VirtualFile { get; }

        public string Content {
            get { return _content; }
            set {
                if (_content == value) {
                    return;
                }

                lock (_writeLock) {
                    Interlocked.Exchange(ref _content, value);
                }
            }
        }

        public string Get() {
            return Content;
        }

        public void Dispose() {}
    }
}