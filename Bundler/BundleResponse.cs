using System;
using System.Linq;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public class BundleResponse : IBundleResponse {
        private readonly Container _container;

        public BundleResponse(string contentType, int version, DateTime lastModification, string content, Container _container) {
            this._container = _container;
            ContentType = contentType;
            Version = version;
            LastModification = lastModification;
            Content = content;
        }

        public string ContentType { get; }
        public int Version { get; }
        public DateTime LastModification { get; }
        public string Content { get; }

        public string[] BundleFiles => _container.GetFiles().ToArray();
        public string GetFile(string virtualFile) => _container.GetFile(virtualFile);
    }
}