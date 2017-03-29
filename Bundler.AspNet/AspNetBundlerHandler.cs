using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public sealed class AspNetBundlerHandler : IHttpHandler {
        public const string IfModifiedSinceHeader = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";

        private readonly IBundle _bundle;
        private readonly string _requestFile;

        public bool IsReusable { get; } = false;

        public AspNetBundlerHandler(IBundle bundle, string requestFile) {
            _bundle = bundle;
            _requestFile = requestFile;
        }

        public void ProcessRequest(HttpContext context) {
            var bundleResponse = _bundle.GetResponse();

            IBundleContent bundleContent;
            if (string.IsNullOrWhiteSpace(_requestFile)) {
                bundleContent = bundleResponse;
            } else if (!bundleResponse.Files.TryGetValue(_requestFile, out bundleContent)) {
                context.Response.StatusCode = 404;
                return;
            }

            if (_bundle.Context.Cache) {
                if (_bundle.Context.ETag) {
                    var requestETag = context.Request.Headers[IfNoneMatch];
                    if (!string.IsNullOrWhiteSpace(requestETag) && string.Equals(requestETag, bundleContent.ContentHash, StringComparison.InvariantCultureIgnoreCase)) {
                        context.Response.StatusCode = 304;
                        return;
                    }
                }

                DateTime requestLastModification;
                var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
                if (!string.IsNullOrWhiteSpace(lastModificationRaw) && DateTime.TryParse(lastModificationRaw, out requestLastModification) && requestLastModification > bundleContent.LastModification) {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.ContentType = bundleResponse.ContentType;
            context.Response.Write(bundleContent.Content);

            context.Response.StatusCode = 200;

            if (_bundle.Context.Cache) {
                if (_bundle.Context.ETag) {
                    context.Response.Cache.SetETag(bundleContent.ContentHash);
                }

                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetLastModified(DateTime.Now);
                context.Response.Cache.SetExpires(DateTime.Now.Add(_bundle.Context.CacheDuration));
            }
        }
    }
}