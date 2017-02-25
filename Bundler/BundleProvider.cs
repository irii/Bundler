using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleProvider : IBundleProvider {
        public IBundleContext Context { get; }

        private readonly object _currentBundleMappingsWriteLock = new object();
        private BundleMappings _currentBundleMappings = BundleMappings.Empty();

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
            if (string.IsNullOrEmpty(virtualPath))
                throw new ArgumentException("Value cannot be null or empty.", nameof(virtualPath));

            return _currentBundleMappings.Paths.TryGetValue(PrepareVirtualPath(virtualPath), out bundle);
        }

        public bool Exists(string virtualPath) {
            if (string.IsNullOrEmpty(virtualPath))
                throw new ArgumentException("Value cannot be null or empty.", nameof(virtualPath));

            return _currentBundleMappings.Paths.ContainsKey(PrepareVirtualPath(virtualPath));
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

        public string Render(string virtualPath) {
            IBundle bundle;
            if (!Get(virtualPath, out bundle)) {
                return string.Empty;
            }

            var response = bundle.GetResponse();

            if (bundle.Context.BundleFiles) {
                return string.Format(bundle.TagFormat, VirtualPathUtility.ToAbsolute(virtualPath) + "?v=" + response.Version);
            }

            var sB = new StringBuilder();
            foreach (var file in response.BundleFiles) {
                var content = response.GetFile(file);
                if (content == null) {
                    continue;
                }
                
                sB.AppendLine(string.Format(bundle.TagFormat, VirtualPathUtility.ToAbsolute(virtualPath) + "?v=" + response.Version + "&f=" + Uri.EscapeDataString(file)));
            }

            return sB.ToString();
        }

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
