using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerHandler : IHttpHandler {
        private readonly IBundle _bundle;

        public bool IsReusable { get; } = false;

        public BundlerHandler(IBundle bundle) {
            _bundle = bundle;
        }


        public void ProcessRequest(HttpContext context) {
            DateTime lastModification;
            if (DateTime.TryParseExact(context.Request.Headers["If-Modified-Since"], "r", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None , out lastModification) && lastModification > _bundle.LastModification) {
                // 304
                context.Response.StatusCode = 304;
                return;
            }

            context.Response.Write(_bundle.Content);
            context.Response.ContentType = _bundle.ContentType;
            context.Response.StatusCode = 200;
            
            context.Response.Cache.SetLastModified(_bundle.LastModification);
            context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
        }

    }
}