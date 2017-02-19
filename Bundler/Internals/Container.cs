using System;
using System.Collections.Generic;
using System.Threading;

namespace Bundler.Internals {
    public sealed class Container {
        private readonly string _placeholder;
        private readonly HashSet<string> _identifiers = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _versionReaderWriterLock = new ReaderWriterLockSlim();

        private string _current = string.Empty;
        private int _version = string.Empty.GetHashCode();

        public Container(string placeholder) {
            _placeholder = placeholder;
        }

        public bool Exists(string identifier) {
            _readerWriterLock.EnterReadLock();
            try {
                return _identifiers.Contains(identifier);
            } finally {
                _readerWriterLock.ExitReadLock();
            }
        }

        public void Append(string identifier, string transformedContent) {
            _readerWriterLock.EnterWriteLock();
            _versionReaderWriterLock.EnterWriteLock();

            if (_identifiers.Contains(identifier)) {
                return;
            }

            _identifiers.Add(identifier);
            _current = string.IsNullOrWhiteSpace(_current) 
                ? string.Concat(_current, transformedContent) 
                : string.Concat(_current, _placeholder, transformedContent);

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