using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler {
    public class BundleProvider : IBundleProvider {
        protected const string VersionQueryParameterName = "v";
        protected const string FileQueryParameterName = "f";


        private readonly object _currentBundleMappingsWriteLock = new object();
        private BundleMappings _currentBundleMappings = BundleMappings.Empty();

        public IBundleContext Context { get; }

        public BundleProvider(IBundleContext context) {
            Helper.ConfigurationValidator.Validate(context);
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

        public bool GetResponse(Uri uri, out IBundleContentResponse bundleContentResponse, out string requestVersion) {
            IBundle bundle;
            if (!Get(uri.AbsolutePath, out bundle)) {
                bundleContentResponse = null;
                requestVersion = null;
                return false;
            }

            var response = bundle.GetResponse();

            var queryArgs = bundle.Context.UrlHelper.ParseQuery(uri.Query);
            queryArgs.TryGetValue(VersionQueryParameterName, out requestVersion);
            if (string.IsNullOrWhiteSpace(requestVersion)) {
                requestVersion = null;
            }

            string requestFile;
            if (queryArgs.TryGetValue(FileQueryParameterName, out requestFile) && !string.IsNullOrWhiteSpace(requestFile)) {
                return response.Files.TryGetValue(requestFile, out bundleContentResponse);
            }

            bundleContentResponse = response;
            return true;
        }

        public string Render(string virtualPath) {
            IBundle bundle;
            if (!Get(virtualPath, out bundle)) {
                return string.Empty;
            }

            var response = bundle.GetResponse();
            
            if (bundle.Context.Configuration.Get(BundlingConfiguration.CombineResponse)) {
                return bundle.Render(GenerateUrl(virtualPath, bundle, response, null));
            }
            
            var sB = new StringBuilder();
            foreach (var file in response.Files) {
                sB.AppendLine(bundle.Render(GenerateUrl(virtualPath, bundle, file.Value, file.Key)));
            }

            return sB.ToString();
        }

        private static string GenerateUrl(string virtualPath, IBundle bundle, IBundleContentResponse response, string fileName = null) {
            var fQuery = !string.IsNullOrWhiteSpace(fileName)
                ? $"{FileQueryParameterName}={bundle.Context.UrlHelper.Encode(fileName)}"
                : null;

            var vQuery = bundle.Context.Configuration.Get(BundlingConfiguration.IncludeContentHash) 
                ? $"{VersionQueryParameterName}={bundle.Context.UrlHelper.Encode(response.ContentHash)}" 
                : null;

            var queryArgs = string.Join("&", new[] {
                fQuery,
                vQuery
            }.Where(x => x != null));

            return string.Join("?", bundle.Context.UrlHelper.ToAbsolute(virtualPath), queryArgs);
        }

        private static void ValidateVirtualPath(string virtualPath) {
            if (virtualPath == null) {
                throw new ArgumentNullException(nameof(virtualPath));
            }

            if (!virtualPath.StartsWith("~/")) {
                throw new ArgumentException("Path should be virtual!");
            }

            if (virtualPath.Contains("?")) {
                throw new ArgumentException("Query arguments are not allowed!");
            }

            if (virtualPath.Contains("../")) {
                throw new ArgumentException("Only normalized paths are allowed.");
            }
        }

        private static string PrepareVirtualPath(string virtualPath) {
            if (virtualPath.Length == 0 || virtualPath[0] == '~') {
                return virtualPath;
            }

            return '~' + virtualPath;
        }

        public IEnumerable<IBundle> GetBundles() => _currentBundleMappings.Paths.Values;

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
