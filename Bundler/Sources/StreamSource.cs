using System;
using System.Collections.Generic;
using System.IO;
using Bundler.Infrastructure;

namespace Bundler.Sources {
    public class StreamSource : ISource {
        public StreamSource(string virtualFile) {
            Identifier = virtualFile;
        }

        public void Dispose() { }

        public bool AddItems(IBundleContext bundleContext, ICollection<ISourceItem> items, ICollection<string> watchPaths) {
            items.Add(new StreamSourceItem(Identifier, bundleContext.VirtualPathProvider));
            watchPaths.Add(Identifier);

            return true;
        }

        public string Identifier { get; }

        private class StreamSourceItem : ISourceItem {
            private readonly IBundleVirtualPathProvider _virtualPathProvider;

            public StreamSourceItem(string virtualFile, IBundleVirtualPathProvider virtualPathProvider) {
                _virtualPathProvider = virtualPathProvider;
                VirtualFile = virtualFile;
            }

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

            public void Dispose() {}
        }
    }
}