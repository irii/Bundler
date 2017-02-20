using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    public static class BundleStore {
        private class MappingTuple {
            public readonly Dictionary<string, Bundle> Keys;
            public readonly Dictionary<string, Bundle> Paths;

            public MappingTuple(Dictionary<string, Bundle> keys, Dictionary<string, Bundle> paths) {
                Keys = keys;
                Paths = paths;
            }
        }
        
        private static readonly IEqualityComparer<string> KeyComparer = StringComparer.InvariantCultureIgnoreCase;
        private static readonly object WriteLock = new object();

        private static MappingTuple _mappings = new MappingTuple(new Dictionary<string, Bundle>(), new Dictionary<string, Bundle>(KeyComparer));

        public static Bundle RegisterKey(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            Bundle bundle;
            if (_mappings.Keys.TryGetValue(bundleKey, out bundle)) {
                return bundle;
            }

            lock (WriteLock) {

                if (_mappings.Keys.TryGetValue(bundleKey, out bundle)) {
                    return bundle;
                }

                bundle = new Bundle(bundleKey, virtualPath, contentBundler);

                var newKeyDictionary = new Dictionary<string, Bundle>(_mappings.Keys);
                var newPathDictionary = new Dictionary<string, Bundle>(_mappings.Paths, KeyComparer);

                newKeyDictionary[bundleKey] = bundle;
                newPathDictionary[virtualPath] = bundle;

                var @new = new MappingTuple(newKeyDictionary, newPathDictionary);
                Interlocked.Exchange(ref _mappings, @new);

                return bundle;
            }
        }

        public static bool GetBundleByPath(string virtualPath, out Bundle bundle) {
            return _mappings.Paths.TryGetValue(virtualPath, out bundle);
        }

        public static bool GetBundleByKey(string bundleKey, out Bundle bundle) {
            return _mappings.Keys.TryGetValue(bundleKey, out bundle);
        }

        public static bool IsBundleKeyRegistered(string bundleKey) {
            return _mappings.Keys.ContainsKey(bundleKey);
        }
    }
}
