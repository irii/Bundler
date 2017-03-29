using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleProvider : IBundleProvider {
        private readonly object _currentBundleMappingsWriteLock = new object();
        private BundleMappings _currentBundleMappings = BundleMappings.Empty();

        public IBundleContext Context { get; }

        public BundleProvider(IBundleContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
        }

        public bool Add(string virtualPath, IBundle bundle) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));

            ValidateVirtualPath(virtualPath);

            if (_currentBundleMappings.Paths.ContainsKey(virtualPath)) {
                return false;
            }

            lock (_currentBundleMappingsWriteLock) {
                if (_currentBundleMappings.Paths.ContainsKey(virtualPath)) {
                    return false;
                }

                var newPathDictionary = _currentBundleMappings.CreatePathDictionary();
                newPathDictionary[virtualPath] = bundle;

                var @new = new BundleMappings(newPathDictionary);
                Interlocked.Exchange(ref _currentBundleMappings, @new);

                return true;
            }
        }

        public bool Get(string virtualPath, out IBundle bundle) {
            if (string.IsNullOrEmpty(virtualPath)) {
                throw new ArgumentException("Value cannot be null or empty.", nameof(virtualPath));
            }

            return _currentBundleMappings.Paths.TryGetValue(PrepareVirtualPath(virtualPath), out bundle);
        }

        public bool Exists(string virtualPath) {
            if (string.IsNullOrEmpty(virtualPath)) {
                throw new ArgumentException("Value cannot be null or empty.", nameof(virtualPath));
            }

            return _currentBundleMappings.Paths.ContainsKey(PrepareVirtualPath(virtualPath));
        }

        public bool ResolveUri(Uri uri, out IBundle bundle, out string requestFile) {
            if (!Get(uri.AbsolutePath, out bundle)) {
                requestFile = null;
                return false;
            }

            var query = uri.Query ?? string.Empty;
            if (query.Length > 0 && query[0] == '?') {
                var splitResult = query.Split(new []{'&'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in splitResult) {
                    var keyValue = s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length != 2 || (!string.Equals(keyValue[0], "f", StringComparison.InvariantCultureIgnoreCase))) {
                        continue;
                    }

                    requestFile = Uri.UnescapeDataString(keyValue[1])?.Trim();
                    if (string.IsNullOrWhiteSpace(requestFile)) {
                        requestFile = null;
                    }

                    return true;
                }
            }

            requestFile = null;
            return true;
        }

        public string Render(string virtualPath, ToAbsoluteDelegate toAbsolute) {
            IBundle bundle;
            if (!Get(virtualPath, out bundle)) {
                return string.Empty;
            }

            var response = bundle.GetResponse();

            if (bundle.Context.BundleFiles) {
                return string.Format(bundle.TagFormat, toAbsolute(virtualPath));
            }

            var sB = new StringBuilder();
            foreach (var file in response.Files) {
                sB.AppendLine(string.Format(bundle.TagFormat, toAbsolute(virtualPath) + "?f=" + Uri.EscapeDataString(file.Key)));
            }

            return sB.ToString();
        }


        private static void ValidateVirtualPath(string virtualPath) {
            if (!virtualPath.StartsWith("~/")) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (virtualPath.Contains("?")) {
                throw new ArgumentException("Query arguments are not allowed!");
            }
        }

        private static string PrepareVirtualPath(string virtualPath) {
            if (virtualPath.Length == 0 || virtualPath[0] == '~') {
                return virtualPath;
            }

            return '~' + virtualPath;
        }

        public IReadOnlyCollection<IBundle> GetBundles() => _currentBundleMappings.Paths.Values.ToList();

        private class BundleMappings {
            private static readonly IEqualityComparer<string> PathComparer = StringComparer.InvariantCultureIgnoreCase;

            public readonly Dictionary<string, IBundle> Paths;

            public BundleMappings(Dictionary<string, IBundle> paths) {
                Paths = paths;
            }

            public static BundleMappings Empty() => new BundleMappings(new Dictionary<string, IBundle>(PathComparer));

            public Dictionary<string, IBundle> CreatePathDictionary()
                => new Dictionary<string, IBundle>(Paths, PathComparer);
        }

    }
}
