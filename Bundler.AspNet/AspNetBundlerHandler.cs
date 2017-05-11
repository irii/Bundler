using System;
using System.IO.Compression;
using System.Web;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

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
            if (_bundleContext.Configuration.Get(CachingConfiguration.Enabled)) {
                // Try Resolve request by IfModifiedSince (requires version tag) Or IfNoneMatch (If Etag is enabled)
                if (TryResolveCacheResponse(context) || TryResolveCacheResponseByETag(context)) {
                    WriteCacheResponse(context);
                    return;
                }
            }

            SetupCompressionFilter(context);
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
            if (!_bundleContext.Configuration.Get(CachingConfiguration.UseEtag)) {
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

        private void WriteCacheResponse(HttpContext context) {
            SetResponseCacheHeaders(context);

            context.Response.StatusCode = 304;
            context.Response.SuppressContent = true;
        }

        private void SetResponseCacheHeaders(HttpContext context) {
            if (_bundleContext.Configuration.Get(CachingConfiguration.UseEtag)) {
                context.Response.Cache.SetETag(_bundleContentResponse.ContentHash);
            }

            var lastModified = DateTime.UtcNow;

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetLastModified(lastModified);
            context.Response.Cache.SetExpires(lastModified.Add(_bundleContext.Configuration.Get(CachingConfiguration.Duration)));
        }


        private void WriteResponse(HttpContext context) {
            context.Response.ContentType = _bundleContentResponse.ContentType;
            context.Response.Write(_bundleContentResponse.Content);
            context.Response.StatusCode = 200;

            foreach (var header in _bundleContentResponse.Headers) {
                context.Response.Headers[header.Key] = header.Value;
            }

            if (_bundleContext.Configuration.Get(CachingConfiguration.Enabled)) {
                SetResponseCacheHeaders(context);
            }
        }

        private void SetupCompressionFilter(HttpContext context) {
            var allowedEncodings = _bundleContext.Configuration.Get(CompressionConfiguration.CompressionAlgorithm);
            if (allowedEncodings == CompressionAlgorithm.None) {
                return;
            }
            
            // TODO: Add weight
            var supportedEncodings = context.Response.Headers["Accept-Encoding"]?.ToLower() ?? string.Empty;
            if (allowedEncodings.HasFlag(CompressionAlgorithm.Gzip) && supportedEncodings.Contains("gzip")) {
                context.Response.Filter = new GZipStream(context.Response.Filter, _bundleContext.Configuration.Get(CompressionConfiguration.CompressionLevel));
                context.Response.AppendHeader("Content-Encoding", "gzip");
            } else if (allowedEncodings.HasFlag(CompressionAlgorithm.Deflate) && supportedEncodings.Contains("deflate")) {
                context.Response.Filter = new DeflateStream(context.Response.Filter, _bundleContext.Configuration.Get(CompressionConfiguration.CompressionLevel));
                context.Response.AppendHeader("Content-Encoding", "deflate");
            }
        }
    }
}