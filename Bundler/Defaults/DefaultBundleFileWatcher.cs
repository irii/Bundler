using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    /// <summary>
    /// TODO: Do not depend on physical location (virtual directories...)
    /// </summary>
    public class DefaultBundleFileWatcher : IBundleFileWatcher {
        private static readonly int ResetCounter = 4;

        private readonly IBundleVirtualPathProvider _virtualPathProvider;
        private readonly FileSystemWatcher _fileSystemWatcher;

        private readonly Task _watchEventGroupingTask;
        private readonly object _fileSyncLock = new object();
        private readonly IDictionary<string, FileChangeEventInfo> _files = new Dictionary<string, FileChangeEventInfo>(StringComparer.InvariantCultureIgnoreCase);
        
        public DefaultBundleFileWatcher(IBundleVirtualPathProvider virtualPathProvider) {
            _virtualPathProvider = virtualPathProvider;
            _fileSystemWatcher = new FileSystemWatcher(virtualPathProvider.GetPhysicalPath("~/")) {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite
            };

            _fileSystemWatcher.Changed += FileSystemWatcher_Event;
            _fileSystemWatcher.EnableRaisingEvents = true;

            _watchEventGroupingTask = Task.Run(new Action(GroupingTask));
        }

        private void FileSystemWatcher_Event(object sender, FileSystemEventArgs e) {
            lock (_fileSyncLock) {
                FileChangeEventInfo fileChangeEventInfo;
                if (_files.TryGetValue(e.FullPath, out fileChangeEventInfo)) {
                    fileChangeEventInfo.SetChangedState();
                }
            }
        }

        public void InvokeChange(string virtualPath) {
            lock (_fileSyncLock) {
                var fullPath = _virtualPathProvider.GetPhysicalPath(virtualPath);

                FileChangeEventInfo fileChangeEventInfo;
                if (_files.TryGetValue(fullPath, out fileChangeEventInfo)) {
                    fileChangeEventInfo.SetChangedState();
                }
            }
        }

        private void GroupingTask() {
            while (_fileSystemWatcher.EnableRaisingEvents) {
                ICollection<FileChangeEventInfo> files;
                lock (_fileSyncLock) {
                    files = _files.Values.ToList();
                }

                foreach (var fileChangeEventInfo in files.Where(x => x.Counter > 0)) {
                    fileChangeEventInfo.DecreaseCounter();

                    if (fileChangeEventInfo.Counter > 0) {
                        continue;
                    }

                    foreach (var subscriber in fileChangeEventInfo.Subscribers) {
                        subscriber(fileChangeEventInfo.VirtualPath);
                        Debug.WriteLine("REFRESHING...");
                    }
                }

                Thread.Sleep(250);
            }
        }

        public void Watch(string virtualPath, FileChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_files) {
                FileChangeEventInfo fileChangeEventInfo;

                var physicalPath = _virtualPathProvider.GetPhysicalPath(virtualPath);

                if (!_files.TryGetValue(physicalPath, out fileChangeEventInfo)) {
                    _files.Add(physicalPath, fileChangeEventInfo = new FileChangeEventInfo(virtualPath, ResetCounter));
                }

                fileChangeEventInfo.Add(callback);
            }
        }

        public void Unwatch(string virtualPath, FileChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_files) {
                var physicalPath = _virtualPathProvider.GetPhysicalPath(virtualPath);

                FileChangeEventInfo fileChangeEventInfo;
                if (!_files.TryGetValue(physicalPath, out fileChangeEventInfo)) {
                    return;
                }

                fileChangeEventInfo.Remove(callback);

                if (fileChangeEventInfo.Subscribers.Count == 0) {
                    _files.Remove(physicalPath);
                }
            }
        }

        public void Dispose() {
            lock (_fileSyncLock) {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _watchEventGroupingTask.Wait();

                _fileSystemWatcher.Dispose();
                _watchEventGroupingTask.Dispose();

                foreach (var fileChangeEventInfo in _files.Values) {
                    fileChangeEventInfo.Dispose();
                }

                _files.Clear();
            }
        }

        private class FileChangeEventInfo : IDisposable {
            private readonly int _resetCounter;
            private readonly object _writeLock = new object();

            private readonly List<FileChangedDelegate> _subscribers = new List<FileChangedDelegate>();
            public IReadOnlyCollection<FileChangedDelegate> Subscribers => _subscribers;

            public string VirtualPath { get; }


            public FileChangeEventInfo(string virtualPath, int resetCounter) {
                _resetCounter = resetCounter;
                VirtualPath = virtualPath;
            }

            public void Add(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    if (!_subscribers.Contains(fileChangedDelegate)) {
                        _subscribers.Add(fileChangedDelegate);
                    }
                }
            }

            public void Remove(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    _subscribers.Remove(fileChangedDelegate);
                }
            }

            public int Counter => _counter;
            private int _counter;

            public void SetChangedState() {
                Interlocked.Add(ref _counter, _resetCounter - (_counter < 0 ? 0 : _counter));
            }

            public void DecreaseCounter() {
                Interlocked.Decrement(ref _counter);
            }

            public void Dispose() {
                lock (_writeLock) {
                    _subscribers.Clear();
                }
            }
        }
    }
}
