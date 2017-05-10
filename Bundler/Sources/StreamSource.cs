using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

namespace Bundler.Sources {
    public class StreamSource : ISource {
        public const string Tag = nameof(StreamSource);

        private readonly string[] _virtualFiles;

        public StreamSource(params string[] virtualFiles) {
            _virtualFiles = virtualFiles;
            Identifier = string.Join(";", virtualFiles.OrderBy(x => x.ToLower()));
        }

        public void Dispose() { }

        public bool AddItems(IBundleContext bundleContext, ICollection<ISourceItem> items, ICollection<string> watchPaths) {
            foreach (var virtualFile in _virtualFiles) {
                if (!virtualFile.StartsWith("~/", StringComparison.InvariantCultureIgnoreCase)) {
                    bundleContext.Diagnostic.Log(LogLevel.Error, Tag, nameof(AddItems), $"Path should be virtual for ${virtualFile}!");
                    return false;
                }

                if (!bundleContext.VirtualPathProvider.FileExists(virtualFile)) {
                    bundleContext.Diagnostic.Log(LogLevel.Error, Tag, nameof(AddItems), $"Can't find file {virtualFile}");
                    return false;
                }

                items.Add(new StreamSourceItem(virtualFile, bundleContext.VirtualPathProvider));
                watchPaths.Add(virtualFile);
            }

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