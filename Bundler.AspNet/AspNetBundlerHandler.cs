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
            if (_bundleContext.Configuration.Cache) {
                // Try Resolve request by IfModifiedSince (requires version tag) Or IfNoneMatch (If Etag is enabled)
                if (TryResolveCacheResponse(context) || TryResolveCacheResponseByETag(context)) {
                    WriteCacheResponse(context);
                    return;
                }
            }

            SetResponseHeaders(context);
            WriteResponse(context);
        }

        private bool TryResolveCacheResponse(HttpContext context) {
            if (string.IsNullOrWhiteSpace(_requestVersion) || !string.Equals(_requestVersion, _bundleContentResponse.ContentHash, StringComparison.InvariantCultureIgnoreCase)) {
                return false;
            }
            
            // Allow caching only if at least last modification is correctly set.
            DateTimeOffset requestLastModification;
            var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
            var isCachingValid = !string.IsNullOrWhiteSpace(lastModificationRaw)
                && DateTimeOffset.TryParse(lastModificationRaw, out requestLastModification)
                && requestLastModification > _bundleContentResponse.LastModification;

            return isCachingValid;
        }

        private bool TryResolveCacheResponseByETag(HttpContext context) {
            if (!_bundleContext.Configuration.ETag) {
                return false; // Disabled
            }

            var requestETags = context.Request.Headers.GetValues(IfNoneMatch);
            if (requestETags == null || requestETags.Length <= 0) {
                return false; // No ETag sent -> Disable
            }

            // Only one ETag is supported.
            if (requestETags.Length != 1) {
                return false; // Not supported -> PreCondition failed
            }
            
            return string.Equals(requestETags[0], _bundleContentResponse.ContentHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private static void WriteCacheResponse(HttpContext context) {
            context.Response.StatusCode = 304;
            context.Response.SuppressContent = true;
        }

        private void WriteResponse(HttpContext context) {
            context.Response.Write(_bundleContentResponse.Content);
            context.Response.StatusCode = 200;
        }

        private void SetResponseHeaders(HttpContext context) {
            context.Response.ContentType = _bundleContentResponse.ContentType;

            if (_bundleContext.Configuration.Cache) {
                if (_bundleContext.Configuration.ETag) {
                    context.Response.Cache.SetETag(_bundleContentResponse.ContentHash);
                }

                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetLastModified(_bundleContentResponse.LastModification.UtcDateTime);
                context.Response.Cache.SetExpires(DateTime.UtcNow.Add(_bundleContext.Configuration.CacheDuration));
            }
        }
    }
}