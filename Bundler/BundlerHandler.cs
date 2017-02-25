using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerHandler : IHttpHandler {
        public const string IfModifiedSinceHeader = "If-Modified-Since";

        private readonly IBundle _bundle;

        public bool IsReusable { get; } = false;

        public BundlerHandler(IBundle bundle) {
            _bundle = bundle;
        }
        
        public void ProcessRequest(HttpContext context) {
            var bundleResponse = _bundle.GetResponse();

            if (_bundle.Context.Cache) {
                DateTime lastModification;
                var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
                if (!string.IsNullOrWhiteSpace(lastModificationRaw) && DateTime.TryParse(lastModificationRaw, out lastModification) && lastModification > bundleResponse.LastModification) {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.ContentType = bundleResponse.ContentType;

            var fQuery = context.Request.QueryString["f"];
            if (!string.IsNullOrWhiteSpace(fQuery)) {
                var content = bundleResponse.GetFile(fQuery);
                if (content == null) {
                    context.Response.StatusCode = 404;
                    return;
                }
                
                context.Response.Write(content);
            } else {
                context.Response.Write(bundleResponse.Content);
            }

            context.Response.StatusCode = 200;

            if (_bundle.Context.Cache) {
                context.Response.Cache.SetLastModified(bundleResponse.LastModification);
                context.Response.Cache.SetExpires(DateTime.Now.Add(_bundle.Context.CacheDuration));
            }
        }
    }
}