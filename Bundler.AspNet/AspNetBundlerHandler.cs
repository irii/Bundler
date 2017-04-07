using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public sealed class AspNetBundlerHandler : IHttpHandler {
        public const string IfModifiedSinceHeader = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";

        private readonly IBundleContext _bundleContext;
        private readonly IBundleContentResponse _bundleContentResponse;
        private readonly string _requestVersion;

        public bool IsReusable { get; } = false;

        public AspNetBundlerHandler(IBundleContext bundleContext, IBundleContentResponse bundleContentResponse, string requestVersion) {
            _bundleContext = bundleContext;
            _bundleContentResponse = bundleContentResponse;
            _requestVersion = requestVersion;
        }

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = _bundleContentResponse.ContentType;

            if (_bundleContext.Configuration.Cache) {
                if (_bundleContext.Configuration.ETag) {
                    var requestETag = context.Request.Headers[IfNoneMatch];
                    if (!string.IsNullOrWhiteSpace(requestETag) && string.Equals(requestETag, _bundleContentResponse.ContentHash, StringComparison.InvariantCultureIgnoreCase)) {
                        context.Response.StatusCode = 304;
                        return;
                    }
                }

                DateTime requestLastModification;
                var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
                if (!string.IsNullOrWhiteSpace(lastModificationRaw) && DateTime.TryParse(lastModificationRaw, out requestLastModification) && requestLastModification > _bundleContentResponse.LastModification) {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.Write(_bundleContentResponse.Content);
            context.Response.StatusCode = 200;

            if (_bundleContext.Configuration.Cache) {
                if (_bundleContext.Configuration.ETag) {
                    context.Response.Cache.SetETag(_bundleContentResponse.ContentHash);
                }

                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetLastModified(DateTime.UtcNow);
                context.Response.Cache.SetExpires(DateTime.UtcNow.Add(_bundleContext.Configuration.CacheDuration));
            }
        }
    }
}