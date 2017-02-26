using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    public sealed class Container {
        private readonly string _placeholder;
        private readonly object _writeLock = new object();

        private Tuple<string, int, DateTime, Dictionary<string, IBundleFile>> _current = new Tuple<string, int, DateTime, Dictionary<string, IBundleFile>>(string.Empty, string.Empty.GetHashCode(), DateTime.Now, CreateDictionary(new Dictionary<string, IBundleFile>(0)));

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Item4.ContainsKey(identifier);
        }
        
        public IReadOnlyDictionary<string, IBundleFile> GetFiles() {
            return _current.Item4;
        }

        public void Append(string virtualFile, string transformedContent) {
            if (string.IsNullOrWhiteSpace(transformedContent)) {
                return;
            }

            if (_current.Item4.ContainsKey(virtualFile)) {
                return;
            }

            lock (_writeLock) {
                if (_current.Item4.ContainsKey(virtualFile)) {
                    return;
                }

                var newContent = string.IsNullOrWhiteSpace(_current.Item1)
                    ? string.Concat(_current.Item1, transformedContent)
                    : string.Concat(_current.Item1, _placeholder, transformedContent);

                var dictionary = CreateDictionary(_current.Item4);
                dictionary.Add(virtualFile, new BundleFile(virtualFile, transformedContent.GetHashCode(), transformedContent));
                
                var @new = new Tuple<string, int, DateTime, Dictionary<string, IBundleFile>>(newContent, newContent.GetHashCode(), DateTime.Now, dictionary);

                Interlocked.Exchange(ref _current, @new);
            }
        }

        public string Content => _current.Item1;
        public DateTime LastModification => _current.Item3;
        public int Version => _current.Item2;


        private static Dictionary<string, IBundleFile> CreateDictionary(IDictionary<string, IBundleFile> values) {
            return new Dictionary<string, IBundleFile>(values, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}