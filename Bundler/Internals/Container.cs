using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    internal sealed class Container {
        public class ContainerTuple {
            public ContainerTuple(string content, IReadOnlyDictionary<string, IBundleContent> files, IReadOnlyDictionary<IBundleContent, IContentSource> sources, string hash) {
                Content = content;
                Files = files;
                Sources = sources;
                Hash = hash;
            }

            public string Content { get; }
            public IReadOnlyDictionary<string, IBundleContent> Files { get; }
            public IReadOnlyDictionary<IBundleContent, IContentSource> Sources { get; }

            public string Hash { get; }
            public DateTime LastModification { get; } = DateTime.Now;


            public static ContainerTuple Empty() => new ContainerTuple(string.Empty, new Dictionary<string, IBundleContent>(StringComparer.InvariantCultureIgnoreCase), new Dictionary<IBundleContent, IContentSource>(), GetContentHash(string.Empty));
        }

        private readonly string _placeholder;
        private readonly object _writeLock = new object();

        private ContainerTuple _current = ContainerTuple.Empty();

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            return _current.Files.ContainsKey(identifier);
        }

        public ContainerTuple Current => _current;

        public bool Append(IContentSource contentSource, string transformedContent) {
            if (contentSource == null) throw new ArgumentNullException(nameof(contentSource));
            if (transformedContent == null) throw new ArgumentNullException(nameof(transformedContent));

            if (_current.Files.ContainsKey(contentSource.VirtualFile)) {
                return false;
            }

            lock (_writeLock) {
                if (_current.Files.ContainsKey(contentSource.VirtualFile)) {
                    return false;
                }

                var newContent = string.IsNullOrWhiteSpace(_current.Content)
                    ? string.Concat(_current.Content, transformedContent)
                    : string.Concat(_current.Content, _placeholder, transformedContent);

                var file = new BundleFile(contentSource.VirtualFile, GetContentHash(transformedContent),
                    transformedContent, DateTime.Now);

                var dictionary = CreateDictionary(_current.Files);
                dictionary.Add(contentSource.VirtualFile, file);

                var sources = _current.Sources.ToDictionary(x => x.Key, y => y.Value);
                sources.Add(file, contentSource);

                var @new = new ContainerTuple(newContent, dictionary, sources, GetContentHash(newContent));

                Interlocked.Exchange(ref _current, @new);
            }

            return true;
        }

        public bool Refresh(Func<ContainerTuple, Container, bool> action) {
            lock (_writeLock) {
                var container = new Container(_placeholder);
                if (!action(_current, container)) {
                    // On error prevent bundle curroption.
                    return false;
                }

                var tuple = container.Current;
                Interlocked.Exchange(ref _current, tuple);
                return true;
            }
        }


        public string Content => _current.Content;
        public DateTime LastModification => _current.LastModification;
        public string ContentHash => _current.Hash;


        private static Dictionary<string, IBundleContent> CreateDictionary(IReadOnlyDictionary<string, IBundleContent> values) {
            return values.ToDictionary(x => x.Key, y => y.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        private static string GetContentHash(string input) {
            // TODO: Check for collisions
            return input.GetHashCode().ToString();
        }
    }
}