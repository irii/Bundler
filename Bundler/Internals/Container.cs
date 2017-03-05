using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    internal sealed class Container {
        private readonly string _placeholder;
        private readonly object _writeLock = new object();

        private Tuple<string, string, DateTime, Dictionary<string, IBundleContent>> _current = new Tuple<string, string, DateTime, Dictionary<string, IBundleContent>>(string.Empty, string.Empty.GetHashCode().ToString(), DateTime.Now, CreateDictionary(new Dictionary<string, IBundleContent>(0)));

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Item4.ContainsKey(identifier);
        }

        public IReadOnlyDictionary<string, IBundleContent> GetFiles() {
            return _current.Item4;
        }

        public void Append(string virtualFile, string transformedContent) {
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));
            if (transformedContent == null) throw new ArgumentNullException(nameof(transformedContent));

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
                dictionary.Add(virtualFile, new BundleFile(virtualFile, GetContentHash(transformedContent), transformedContent, DateTime.Now));

                var @new = new Tuple<string, string, DateTime, Dictionary<string, IBundleContent>>(newContent, GetContentHash(newContent), DateTime.Now, dictionary);

                Interlocked.Exchange(ref _current, @new);
            }
        }

        public string Content => _current.Item1;
        public DateTime LastModification => _current.Item3;
        public string ContentHash => _current.Item2;


        private static Dictionary<string, IBundleContent> CreateDictionary(IDictionary<string, IBundleContent> values) {
            return new Dictionary<string, IBundleContent>(values, StringComparer.InvariantCultureIgnoreCase);
        }

        private static string GetContentHash(string input) {
            // TODO: Check for collisions
            return input.GetHashCode().ToString();
        }
    }
}