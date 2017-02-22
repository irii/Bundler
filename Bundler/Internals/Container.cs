using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bundler.Internals {
    public sealed class Container {
        private readonly string _placeholder;
        private readonly object _writeLock = new object();
        private Tuple<string, int, DateTime, HashSet<string>> _current = new Tuple<string, int, DateTime, HashSet<string>>(string.Empty, string.Empty.GetHashCode(), DateTime.Now, CreateHashSet(Enumerable.Empty<string>()));

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Item4.Contains(identifier);
        }

        public void Append(string identifier, string transformedContent) {
            if (string.IsNullOrWhiteSpace(transformedContent)) {
                return;
            }

            if (_current.Item4.Contains(identifier)) {
                return;
            }

            lock (_writeLock) {
                if (_current.Item4.Contains(identifier)) {
                    return;
                }

                var newContent = string.IsNullOrWhiteSpace(_current.Item1)
                    ? string.Concat(_current.Item1, transformedContent)
                    : string.Concat(_current.Item1, _placeholder, transformedContent);

                var hashSet = CreateHashSet(_current.Item4);
                hashSet.Add(identifier);
                
                var @new = new Tuple<string, int, DateTime, HashSet<string>>(newContent, newContent.GetHashCode(), DateTime.Now, hashSet);

                Interlocked.Exchange(ref _current, @new);
            }
        }

        public string Content => _current.Item1;
        public DateTime LastModification => _current.Item3;
        public int Version => _current.Item2;


        private static HashSet<string> CreateHashSet(IEnumerable<string> values) {
            return new HashSet<string>(values, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}