using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    public class FileWatcher : IDisposable {
        private readonly IBundleVirtualPathProvider _virtualPathProvider;
        private readonly object _syncLock = new object();
        private readonly object _queueSyncLock = new object();

        protected FileSystemWatcher FileSystemWatcher { get; }

        private Task WatchEventGroupingTask { get; set; }

        private readonly IDictionary<string, FileChangeEventInfo> _files = new Dictionary<string, FileChangeEventInfo>(StringComparer.InvariantCultureIgnoreCase);

        public FileWatcher(IBundleVirtualPathProvider virtualPathProvider, string virtualPathRoot) {
            _virtualPathProvider = virtualPathProvider;
            FileSystemWatcher = new FileSystemWatcher(virtualPathProvider.GetPhysicalPath(virtualPathRoot)) {
                IncludeSubdirectories = true
            };

            FileSystemWatcher.Changed += FileSystemWatcher_Event;
        }

        private void FileSystemWatcher_Event(object sender, FileSystemEventArgs e) {
            lock (_queueSyncLock) {
                FileChangeEventInfo fileChangeEventInfo;
                if (_files.TryGetValue(e.FullPath, out fileChangeEventInfo)) {
                    fileChangeEventInfo.SetChangedState();
                }
            }
        }

        public bool IsWatching {
            get {
                lock (_syncLock) {
                    return FileSystemWatcher.EnableRaisingEvents && WatchEventGroupingTask != null;
                }
            }
        }

        public void StartWatching() {
            lock (_syncLock) {
                if (WatchEventGroupingTask != null) {
                    FileSystemWatcher.EnableRaisingEvents = false;
                    WatchEventGroupingTask.Wait();
                }

                FileSystemWatcher.EnableRaisingEvents = true;
                WatchEventGroupingTask = Task.Run(new Action(GroupingTask));
            }
        }

        public void StopWatching() {
            lock (_syncLock) {
                FileSystemWatcher.EnableRaisingEvents = false;
                WatchEventGroupingTask?.Wait();
                WatchEventGroupingTask = null;
            }
        }

        public void Watch(string virtualFile, FileChangedDelegate callback) {
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_syncLock) {
                FileChangeEventInfo fileChangeEventInfo;

                var physicalPath = _virtualPathProvider.GetPhysicalPath(virtualFile);

                if (!_files.TryGetValue(physicalPath, out fileChangeEventInfo)) {
                    _files.Add(physicalPath, fileChangeEventInfo = new FileChangeEventInfo(virtualFile, physicalPath));
                }

                fileChangeEventInfo.Add(callback);
            }
        }

        public void Unwatch(string virtualFile, FileChangedDelegate callback) {
            if (virtualFile == null) throw new ArgumentNullException(nameof(virtualFile));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_syncLock) {
                var physicalPath = _virtualPathProvider.GetPhysicalPath(virtualFile);

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

        private void GroupingTask() {
            while (FileSystemWatcher.EnableRaisingEvents) {
                ICollection<FileChangeEventInfo> files;
                lock (_queueSyncLock) {
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

        public void Dispose() {
            lock (_syncLock) {
                StopWatching();
                FileSystemWatcher?.Dispose();
                WatchEventGroupingTask?.Dispose();
            }
        }


        private class FileChangeEventInfo {
            private readonly object _writeLock = new object();

            private readonly HashSet<FileChangedDelegate> _subscribers = new HashSet<FileChangedDelegate>();

            public IReadOnlyCollection<FileChangedDelegate> Subscribers {
                get {
                    lock (_writeLock) {
                        return _subscribers.ToList();
                    }
                }
            }

            public string VirtualPath { get; }
            public string AbsolutePath { get; }

            public void Add(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    if (_subscribers.Contains(fileChangedDelegate)) {
                        return;
                    }

                    _subscribers.Add(fileChangedDelegate);
                }
            }

            public void Remove(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    if (!_subscribers.Contains(fileChangedDelegate)) {
                        return;
                    }

                    _subscribers.Remove(fileChangedDelegate);
                }
            }

            public int Counter => _counter;
            private int _counter;
            public FileChangeEventInfo(string virtualPath, string absolutePath) {
                VirtualPath = virtualPath;
                AbsolutePath = absolutePath;
            }

            public void SetChangedState() {
                while (_counter < 4) {
                    Interlocked.Increment(ref _counter);
                }
            }

            public void DecreaseCounter() {
                Interlocked.Decrement(ref _counter);
            }
        }
    }
}
