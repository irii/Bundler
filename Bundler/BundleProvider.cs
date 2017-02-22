using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleProvider : IBundleProvider {
        public IBundleProviderConfiguration Configuration { get; }

        private readonly object _currentBundleMappingsWriteLock = new object();
        private BundleMappings _currentBundleMappings = BundleMappings.Empty();

        public BundleProvider(IBundleProviderConfiguration configuration) {
            //if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            Configuration = configuration;
        } 

        public bool AddOrGet(string virtualPath, Func<IContentBundler> contentBundlerFactory, out IBundle bundle) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (contentBundlerFactory == null) throw new ArgumentNullException(nameof(contentBundlerFactory));

            ValidateVirtualPath(virtualPath);

            if (_currentBundleMappings.Paths.TryGetValue(virtualPath, out bundle)) {
                return true;
            }

            lock (_currentBundleMappingsWriteLock) {
                if (_currentBundleMappings.Paths.TryGetValue(virtualPath, out bundle)) {
                    return true;
                }

                bundle = new Bundle(virtualPath, contentBundlerFactory());
                var newPathDictionary = _currentBundleMappings.CreatePathDictionary();

                newPathDictionary[virtualPath] = bundle;

                var @new = new BundleMappings(newPathDictionary);
                Interlocked.Exchange(ref _currentBundleMappings, @new);

                return true;
            }
        }

        public bool Add(string virtualPath, IContentBundler contentBundler, out IBundle bundle) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (contentBundler == null) throw new ArgumentNullException(nameof(contentBundler));

            ValidateVirtualPath(virtualPath);

            if (_currentBundleMappings.Paths.ContainsKey(virtualPath)) {
                bundle = null;
                return false;
            }

            lock (_currentBundleMappingsWriteLock) {
                if (_currentBundleMappings.Paths.ContainsKey(virtualPath)) {
                    bundle = null;
                    return false;
                }

                bundle = new Bundle(virtualPath, contentBundler);
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
