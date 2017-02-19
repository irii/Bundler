using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    public static class BundleStore {
        private static readonly ReaderWriterLockSlim PathLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim KeyLock = new ReaderWriterLockSlim();

        private static readonly Dictionary<string, Bundle> PathDictionary = new Dictionary<string, Bundle>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, Bundle> KeyDictionary = new Dictionary<string, Bundle>();

        public static Bundle RegisterKey(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            Bundle bundle;
            KeyLock.EnterReadLock();
            try {
                if (KeyDictionary.TryGetValue(bundleKey, out bundle)) {
                    return bundle;
                }
            }
            finally {
                KeyLock.ExitReadLock();
            }

            bundle = new Bundle(bundleKey, virtualPath, contentBundler);

            KeyLock.EnterWriteLock();
            PathLock.EnterWriteLock();

            try {
                KeyDictionary[bundleKey] = bundle;
                PathDictionary[virtualPath] = bundle;
            }
            finally {
                PathLock.ExitWriteLock();
                KeyLock.ExitWriteLock();
            }

            return bundle;
        }

        public static bool GetBundleByPath(string virtualPath, out Bundle bundle) {
            PathLock.EnterReadLock();
            try {
                return PathDictionary.TryGetValue(virtualPath, out bundle);
            }
            finally {
                PathLock.ExitReadLock();
            }
        }

        public static bool GetBundleByKey(string bundleKey, out Bundle bundle) {
            KeyLock.EnterReadLock();
            try {
                return KeyDictionary.TryGetValue(bundleKey, out bundle);
            } finally {
                KeyLock.ExitReadLock();
            }
        }

        public static bool IsBundleKeyRegistered(string bundleKey) {
            KeyLock.EnterReadLock();
            try {
                return KeyDictionary.ContainsKey(bundleKey);
            } finally {
                KeyLock.ExitReadLock();
            }
        }
    }
}
