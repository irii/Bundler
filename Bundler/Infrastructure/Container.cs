﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Bundler.Infrastructure {
    public sealed class Container {
        private readonly HashSet<string> _virutalFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _versionReaderWriterLock = new ReaderWriterLockSlim();

        private string _current = string.Empty;
        private int _version = string.Empty.GetHashCode();

        public bool Exists(string virtualFile) {
            _readerWriterLock.EnterReadLock();
            try {
                return _virutalFiles.Contains(virtualFile);
            } finally {
                _readerWriterLock.ExitReadLock();
            }
        }

        public void Append(string virtualFile, string transformedContent, string placeholder) {
            _readerWriterLock.EnterWriteLock();
            _versionReaderWriterLock.EnterWriteLock();

            _virutalFiles.Add(virtualFile);
            _current = string.IsNullOrWhiteSpace(_current) 
                ? string.Concat(_current, transformedContent) 
                : string.Concat(_current, placeholder, transformedContent);

            _version = _current.GetHashCode();
            if (_version < 0) {
                _version *= -1;
            }

            _versionReaderWriterLock.ExitWriteLock();
            _readerWriterLock.ExitWriteLock();
        }

        public string Get() {
            _readerWriterLock.EnterReadLock();
            try {
                return _current;
            } finally {
                _readerWriterLock.ExitReadLock();
            }
        }

        public int GetVersion() {
            _versionReaderWriterLock.EnterReadLock();
            try {
                return _version;
            } finally {
                _versionReaderWriterLock.ExitReadLock();
            }
        }
    }
}