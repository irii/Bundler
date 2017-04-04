using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    internal sealed class Container {
        public class ContainerTuple {
            public ContainerTuple(string content, IReadOnlyDictionary<string, IBundleContentResponse> files, IReadOnlyDictionary<IBundleContentResponse, ISource> sources, string hash) {
                Content = content;
                Files = files;
                Sources = sources;
                Hash = hash;
            }

            public string Content { get; }
            public IReadOnlyDictionary<string, IBundleContentResponse> Files { get; }
            public IReadOnlyDictionary<IBundleContentResponse, ISource> Sources { get; }

            public string Hash { get; }
            public DateTime LastModification { get; } = DateTime.Now;


            public static ContainerTuple Empty() => new ContainerTuple(string.Empty, new Dictionary<string, IBundleContentResponse>(StringComparer.InvariantCultureIgnoreCase), new Dictionary<IBundleContentResponse, ISource>(), GetContentHash(string.Empty));
        }

        private readonly string _placeholder;
        private readonly string _contentType;
        private readonly object _writeLock = new object();

        private ContainerTuple _current = ContainerTuple.Empty();

        public Container(string placeholder, string contentType) {
            _placeholder = placeholder;
            _contentType = contentType;
        }

        public bool Exists(string identifier) {
            return _current.Files.ContainsKey(identifier);
        }

        public ContainerTuple Current => _current;

        public bool Append(ISource source, string transformedContent) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (transformedContent == null) throw new ArgumentNullException(nameof(transformedContent));

            if (_current.Files.ContainsKey(source.VirtualFile)) {
                return false;
            }

            lock (_writeLock) {
                if (_current.Files.ContainsKey(source.VirtualFile)) {
                    return false;
                }

                var newContent = string.IsNullOrWhiteSpace(_current.Content)
                    ? string.Concat(_current.Content, transformedContent)
                    : string.Concat(_current.Content, _placeholder, transformedContent);

                var file = new BundleFile(source.VirtualFile, _contentType, GetContentHash(transformedContent), transformedContent, DateTime.Now);

                var dictionary = CreateDictionary(_current.Files);
                dictionary.Add(source.VirtualFile, file);

                var sources = _current.Sources.ToDictionary(x => x.Key, y => y.Value);
                sources.Add(file, source);

                var @new = new ContainerTuple(newContent, dictionary, sources, GetContentHash(newContent));

                Interlocked.Exchange(ref _current, @new);
            }

            return true;
        }

        public bool Refresh(Func<ContainerTuple, Container, bool> action) {
            lock (_writeLock) {
                var container = new Container(_placeholder, _contentType);
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


        private static Dictionary<string, IBundleContentResponse> CreateDictionary(IReadOnlyDictionary<string, IBundleContentResponse> values) {
            return values.ToDictionary(x => x.Key, y => y.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        private static string GetContentHash(string input) {
            using (var sha256 = SHA256.Create()) {
                var inputBytes = Encoding.Unicode.GetBytes(input);
                var result = Convert.ToBase64String(sha256.ComputeHash(inputBytes))
                    .Replace("+", "-").Replace("/", "_");

                var paddingCount = 0;
                var indexOfPadding = result.IndexOf('=');
                if (indexOfPadding >= 0) {
                    paddingCount = result.Length - indexOfPadding;
                }

                return result.Remove(result.Length - paddingCount) + "_" + paddingCount;
            }
        }
    }
}