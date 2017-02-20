using System;
using System.Collections.Generic;
using System.Threading;

namespace Bundler.Internals {
    public sealed class Container {
        private readonly string _placeholder;
        private readonly object _writeLock = new object();
        private Tuple<string, int, HashSet<string>> _current = new Tuple<string, int, HashSet<string>>(string.Empty, string.Empty.GetHashCode(), CreateHashSet());

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Item3.Contains(identifier);
        }

        public void Append(string identifier, string transformedContent) {
            if (_current.Item3.Contains(identifier)) {
                return;
            }

            lock (_writeLock) {
                if (_current.Item3.Contains(identifier)) {
                    return;
                }

                var newContent = string.IsNullOrWhiteSpace(_current.Item1)
                    ? string.Concat(_current.Item1, transformedContent)
                    : string.Concat(_current.Item1, _placeholder, transformedContent);

                var hashSet = CreateHashSet(_current.Item3);
                hashSet.Add(identifier);

                var @new = new Tuple<string, int, HashSet<string>>(newContent, newContent.GetHashCode(), hashSet);

                Interlocked.Exchange(ref _current, @new);
            }
        }

        public string Get() {
            return _current.Item1;
        }

        public int GetVersion() {
            return _current.Item2;
        }


        private static HashSet<string> CreateHashSet() {
            return new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
        private static HashSet<string> CreateHashSet(IEnumerable<string> values) {
            return new HashSet<string>(values, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}