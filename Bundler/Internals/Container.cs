using System;
using System.Collections.Generic;
using System.Threading;

namespace Bundler.Internals {
    public sealed class Container {
        private readonly string _placeholder;
        private readonly object _writeLock = new object();

        private Tuple<string, int, DateTime, Dictionary<string, string>> _current = new Tuple<string, int, DateTime, Dictionary<string, string>>(string.Empty, string.Empty.GetHashCode(), DateTime.Now, CreateDictionary(new Dictionary<string, string>(0)));

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Item4.ContainsKey(identifier);
        }

        public ICollection<string> GetFiles() => _current.Item4.Keys;

        public string GetFile(string virtualFile) {
            string content;
            if (_current.Item4.TryGetValue(virtualFile, out content)) {
                return content ?? string.Empty;
            }

            return null;
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
                dictionary.Add(virtualFile, transformedContent);
                
                var @new = new Tuple<string, int, DateTime, Dictionary<string, string>>(newContent, newContent.GetHashCode(), DateTime.Now, dictionary);

                Interlocked.Exchange(ref _current, @new);
            }
        }

        public string Content => _current.Item1;
        public DateTime LastModification => _current.Item3;
        public int Version => _current.Item2;


        private static Dictionary<string, string> CreateDictionary(IDictionary<string, string> values) {
            return new Dictionary<string, string>(values, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}