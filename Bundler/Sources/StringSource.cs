using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Sources {
    public class StringSource : ISource {
        private readonly object _writeLock = new object();
        private string _content;

        private readonly ISourceItem _stringSourceItem;

        public StringSource(string virtualFile, string content) {
            Content = content;
            Identifier = virtualFile;

            _stringSourceItem = new StringSourceItem(virtualFile, () => Content);
        }

        public string Identifier { get; }

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
        
        public void Dispose() {}

        public bool AddItems(IBundleContext bundleContext, ICollection<ISourceItem> items, ICollection<string> watchPaths) {
            items.Add(_stringSourceItem);
            return true;
        }


        private class StringSourceItem : ISourceItem {
            private readonly Func<string> _getContentFunc;

            public StringSourceItem(string virtualFile, Func<string> getContentFunc) {
                VirtualFile = virtualFile;
                _getContentFunc = getContentFunc;
            }

            public string VirtualFile { get; }
            public string Get() => _getContentFunc();
            public void Dispose() {}
        }
    }
}