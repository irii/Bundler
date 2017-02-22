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
            // TODO: Add cache enable setting.

            DateTime lastModification;
            var lastModificationRaw = context.Request.Headers[IfModifiedSinceHeader];
            if (!string.IsNullOrWhiteSpace(lastModificationRaw) && DateTime.TryParse(lastModificationRaw, out lastModification) && lastModification > _bundle.LastModification) {
                context.Response.StatusCode = 304;
                return;
            }

            context.Response.Write(_bundle.Content);
            context.Response.ContentType = _bundle.ContentType;
            context.Response.StatusCode = 200;

            // TODO: Add cache enable setting.
            context.Response.Cache.SetLastModified(_bundle.LastModification);
            context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
        }

    }
}