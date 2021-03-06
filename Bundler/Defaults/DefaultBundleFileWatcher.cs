﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

namespace Bundler.Defaults {
    /// <summary>
    /// DefaultBundleFileWatcher
    /// </summary>
    public class DefaultBundleFileWatcher : IBundleFileWatcher {
        private const string Tag = nameof(DefaultBundleFileWatcher);

        private static readonly int ResetCounter = 4;

        private readonly IBundleDiagnostic _bundleDiagnostic;
        private readonly IBundleVirtualPathProvider _virtualPathProvider;
        private readonly FileSystemWatcher _fileSystemWatcher;

        private readonly Task _watchEventGroupingTask;
        private readonly object _fileSyncLock = new object();
        private readonly IDictionary<string, FileChangeEventInfo> _files = new Dictionary<string, FileChangeEventInfo>(StringComparer.InvariantCultureIgnoreCase);
        
        public DefaultBundleFileWatcher(IBundleDiagnostic bundleDiagnostic, IBundleVirtualPathProvider virtualPathProvider) {
            _bundleDiagnostic = bundleDiagnostic;
            _virtualPathProvider = virtualPathProvider;
            _fileSystemWatcher = new FileSystemWatcher(virtualPathProvider.GetPhysicalPath("~/")) {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName
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

        private string GetPhysicalPath(string virtualPath) {
            return Path.GetFullPath(_virtualPathProvider.GetPhysicalPath(virtualPath));
        }

        public void InvokeChange(string virtualPath) {
            lock (_fileSyncLock) {
                var fullPath = GetPhysicalPath(virtualPath);

                FileChangeEventInfo fileChangeEventInfo;
                if (_files.TryGetValue(fullPath, out fileChangeEventInfo)) {
                    fileChangeEventInfo.SetChangedState();
                }
            }
        }

        private void GroupingTask() {
            while (_fileSystemWatcher.EnableRaisingEvents) {
                lock (_fileSyncLock) {
                    foreach (var fileChangeEventInfo in _files.Values.Where(x => x.Counter > 0)) {
                        fileChangeEventInfo.DecreaseCounter();

                        if (fileChangeEventInfo.Counter > 0) {
                            continue;
                        }

                        foreach (var subscriber in fileChangeEventInfo.Subscribers) {
                            subscriber(fileChangeEventInfo.VirtualPath);
                        }
                    }
                }

                Thread.Sleep(250);
            }
        }

        public void Watch(string virtualPath, SourceChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_files) {
                FileChangeEventInfo fileChangeEventInfo;

                var physicalPath = GetPhysicalPath(virtualPath);

                if (!_files.TryGetValue(physicalPath, out fileChangeEventInfo)) {
                    _files.Add(physicalPath, fileChangeEventInfo = new FileChangeEventInfo(virtualPath, ResetCounter));
                }

                _bundleDiagnostic.Log(LogLevel.Debug, Tag, nameof(Watch), $"{virtualPath} ({physicalPath}) added for watching.");
                fileChangeEventInfo.Add(callback);
            }
        }

        public void Unwatch(string virtualPath, SourceChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            lock (_files) {
                var physicalPath = GetPhysicalPath(virtualPath);

                FileChangeEventInfo fileChangeEventInfo;
                if (!_files.TryGetValue(physicalPath, out fileChangeEventInfo)) {
                    return;
                }

                fileChangeEventInfo.Remove(callback);
                _bundleDiagnostic.Log(LogLevel.Debug, Tag, nameof(Watch), $"{virtualPath} ({physicalPath}) removed from watching.");

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

            private readonly List<SourceChangedDelegate> _subscribers = new List<SourceChangedDelegate>();
            public IReadOnlyCollection<SourceChangedDelegate> Subscribers => _subscribers;

            public string VirtualPath { get; }


            public FileChangeEventInfo(string virtualPath, int resetCounter) {
                _resetCounter = resetCounter;
                VirtualPath = virtualPath;
            }

            public void Add(SourceChangedDelegate sourceChangedDelegate) {
                lock (_writeLock) {
                    if (!_subscribers.Contains(sourceChangedDelegate)) {
                        _subscribers.Add(sourceChangedDelegate);
                    }
                }
            }

            public void Remove(SourceChangedDelegate sourceChangedDelegate) {
                lock (_writeLock) {
                    _subscribers.Remove(sourceChangedDelegate);
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
