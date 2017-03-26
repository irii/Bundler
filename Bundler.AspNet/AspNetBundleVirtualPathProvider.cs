using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleVirtualPathProvider : IBundleVirtualPathProvider {
        private class FileChangeEventInfo {
            private readonly object _writeLock = new object();

            private List<FileChangedDelegate> _subscribers = new List<FileChangedDelegate>(0);
            public IEnumerable<FileChangedDelegate> Subscribers => _subscribers;

            public string VirtualPath { get; }

            public void Add(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    if (_subscribers.Contains(fileChangedDelegate)) {
                        return;
                    }

                    var newList = new List<FileChangedDelegate>(_subscribers) { fileChangedDelegate };
                    Interlocked.Exchange(ref _subscribers, newList);
                }
            }

            public void Remove(FileChangedDelegate fileChangedDelegate) {
                lock (_writeLock) {
                    if (!_subscribers.Contains(fileChangedDelegate)) {
                        return;
                    }

                    var newList = new List<FileChangedDelegate>(_subscribers.Where(x => x != fileChangedDelegate));
                    Interlocked.Exchange(ref _subscribers, newList);
                }
            }

            public int Counter => _counter;
            private int _counter;
            public FileChangeEventInfo(string virtualPath) {
                VirtualPath = virtualPath;
            }

            public void IncreaseCounter() {
                if (_counter < 0) {
                    _counter = 0;
                }
                Interlocked.Increment(ref _counter);
            }

            public void DecreaseCounter() {
                Interlocked.Decrement(ref _counter);
            }

        }

        private readonly object _subscribersLock = new object();
        private readonly IDictionary<string, FileChangeEventInfo> _subscribers = new Dictionary<string, FileChangeEventInfo>(StringComparer.InvariantCultureIgnoreCase);

        private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        private readonly Task _changeQueueHandler;

        private bool _queueAbort = false;

        public AspNetBundleVirtualPathProvider(bool enableFileSystemWachter, string listenFolder = "~/") {
            _fileSystemWatcher.Path = HostingEnvironment.MapPath(listenFolder ?? "~/");
            _fileSystemWatcher.Filter = "*.*";
            _fileSystemWatcher.IncludeSubdirectories = true;

            _fileSystemWatcher.Created += FileSystemWatcher_Changed;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Changed;

            _fileSystemWatcher.EnableRaisingEvents = enableFileSystemWachter;

            if (enableFileSystemWachter) {
                _changeQueueHandler = Task.Run(() => {
                    while (_queueAbort == false) {
                        ICollection<FileChangeEventInfo> fileFileChangeEventInfos;
                        lock (_subscribersLock) {
                            fileFileChangeEventInfos = _subscribers.Values.ToList();
                        }

                        foreach (var fileFileChangeEventInfo in fileFileChangeEventInfos) {
                            if (fileFileChangeEventInfo.Counter < 0) {
                                continue;
                            }

                            fileFileChangeEventInfo.DecreaseCounter();

                            if (fileFileChangeEventInfo.Counter == 0) {
                                foreach (var fileChangedDelegate in fileFileChangeEventInfo.Subscribers) {
                                    fileChangedDelegate(fileFileChangeEventInfo.VirtualPath);
                                }
                            }
                        }

                        Thread.Sleep(15);
                    }
                });
            }
        }

        public AspNetBundleVirtualPathProvider() : this(true) { }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
            FileChangeEventInfo fileChangeEventInfo;
            lock (_subscribersLock) {
                if (!_subscribers.TryGetValue(e.FullPath, out fileChangeEventInfo)) {
                    return;
                }
            }

            for(var i = 0; i < 5; i++)
                fileChangeEventInfo.IncreaseCounter();
        }

        public Stream Open(string virtualPath) {
            var virtualFile = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);
            if (virtualFile == null) {
                throw new FileNotFoundException("Can't find file", virtualPath);
            }

            return virtualFile.Open();
        }

        public bool FileExists(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
        }

        public bool DirectoryExists(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualPath);
        }

        public IEnumerable<string> EnumerateFiles(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath)
                .Files.OfType<VirtualFileBase>()
                .Select(x => x.VirtualPath)
                .ToList();
        }

        public IEnumerable<string> EnumerateDirectories(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath)
                .Directories.OfType<VirtualFileBase>()
                .Select(x => x.VirtualPath)
                .ToList();
        }

        public bool Watch(string virtualPath, FileChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var absolutePath = HostingEnvironment.MapPath(virtualPath);
            if (absolutePath == null) {
                return false;
            }

            FileChangeEventInfo fileChangeEventInfo;
            lock (_subscribersLock) {
                if (!_subscribers.TryGetValue(absolutePath, out fileChangeEventInfo)) {
                    _subscribers.Add(absolutePath, fileChangeEventInfo = new FileChangeEventInfo(virtualPath));
                }
            }

            fileChangeEventInfo.Add(callback);
            return false;
        }

        public bool Unwatch(string virtualPath, FileChangedDelegate callback) {
            if (virtualPath == null) throw new ArgumentNullException(nameof(virtualPath));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var absolutePath = HostingEnvironment.MapPath(virtualPath);
            if (absolutePath == null) {
                return false;
            }

            FileChangeEventInfo fileChangeEventInfo;
            lock (_subscribersLock) {
                if (!_subscribers.TryGetValue(absolutePath, out fileChangeEventInfo)) {
                    return false;
                }
            }

            fileChangeEventInfo.Remove(callback);
            return true;
        }

        public void Dispose() {
            _fileSystemWatcher?.Dispose();
            _queueAbort = true;
        }

        public string GetFullPath(string virtualPath) {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public string GetVirtualPath(string absolutePath) {
            if (absolutePath == null) throw new ArgumentNullException(nameof(absolutePath));
            absolutePath = absolutePath.Replace("\\", "/");

            var absoluteAppPath = HostingEnvironment.MapPath("~/")?.Replace("\\", "/");
            if (absoluteAppPath == null) {
                throw new Exception("Can't resolve app dir.");
            }

            if (!absolutePath.StartsWith(absoluteAppPath, StringComparison.InvariantCultureIgnoreCase)) {
                throw new Exception("Invalid path.");
            }
            
            return "~/" + absolutePath.Substring(absoluteAppPath.Length);
        }
    }
}