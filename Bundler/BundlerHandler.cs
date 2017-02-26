using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerHandler : IHttpHandler {
        public const string IfModifiedSinceHeader = "If-Modified-Since";

        private readonly IBundle _bundle;
        private readonly int _requestVersion;
        private readonly string _requestFile;

        public bool IsReusable { get; } = false;

        public BundlerHandler(IBundle bundle, int requestVersion, string requestFile) {
            _bundle = bundle;
            _requestVersion = requestVersion;
            _requestFile = requestFile;
        }
        
        public void ProcessRequest(HttpContext context) {
            var bundleResponse = _bundle.GetResponse();

            var isFileRequest = !string.IsNullOrWhiteSpace(_requestFile);

            IBundleFile file = null;
            if (isFileRequest && !bundleResponse.Files.TryGetValue(_requestFile, out file)) {
                context.Response.StatusCode = 404;
                return;
            }
            
            if (_bundle.Context.Cache) {
                var lastModification = isFileRequest 
                    ? file.LastModification 
                    : bundleResponse.LastModification;

                DateTime requestLastModification;
                var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
                if (!string.IsNullOrWhiteSpace(lastModificationRaw) && DateTime.TryParse(lastModificationRaw, out requestLastModification) && requestLastModification > lastModification) {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.ContentType = bundleResponse.ContentType;
            
            if (isFileRequest) {
                context.Response.Write(file.Content);
            } else {
                context.Response.Write(bundleResponse.Content);
            }

            context.Response.StatusCode = 200;

            if (_bundle.Context.Cache) {
                context.Response.Cache.SetCacheability(HttpCacheability.Private);
                context.Response.Cache.SetLastModified(DateTime.Now);
                context.Response.Cache.SetExpires(DateTime.Now.Add(_bundle.Context.CacheDuration));
            }
        }
    }
}