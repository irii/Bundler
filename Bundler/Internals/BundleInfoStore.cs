using System;
using System.Collections.Generic;
using System.Threading;
using Bundler.Infrastructure;

namespace Bundler.Internals {
    public static class BundleInfoStore {
        private static readonly ReaderWriterLockSlim PathLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim KeyLock = new ReaderWriterLockSlim();

        private static readonly Dictionary<string, BundleInfo> PathDictionary = new Dictionary<string, BundleInfo>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, BundleInfo> KeyDictionary = new Dictionary<string, BundleInfo>();

        public static void RegisterKey(string bundleKey, string virtualPath, IContentBundler contentBundler) {
            KeyLock.EnterReadLock();
            try {
                if (KeyDictionary.ContainsKey(bundleKey)) {
                    return;
                }
            }
            finally {
                KeyLock.ExitReadLock();
            }

            var bundleInfo = new BundleInfo(bundleKey, virtualPath, contentBundler);

            KeyLock.EnterWriteLock();
            PathLock.EnterWriteLock();

            try {
                KeyDictionary[bundleKey] = bundleInfo;
                PathDictionary[virtualPath] = bundleInfo;
            }
            finally {
                PathLock.ExitWriteLock();
                KeyLock.ExitWriteLock();
            }
        }

        public static bool GetBundleInfoByPath(string virtualPath, out BundleInfo bundleInfo) {
            PathLock.EnterReadLock();
            try {
                return PathDictionary.TryGetValue(virtualPath, out bundleInfo);
            }
            finally {
                PathLock.ExitReadLock();
            }
        }

        public static bool GetBundleInfoByKey(string bundleKey, out BundleInfo bundleInfo) {
            KeyLock.EnterReadLock();
            try {
                return KeyDictionary.TryGetValue(bundleKey, out bundleInfo);
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
