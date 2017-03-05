using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public class DefaultBundleContext : IBundleContext {
        private static HttpApplication HttpApplication => HttpContext.Current.ApplicationInstance;

        public string GetFullPath(string virtualPath) => HttpApplication.Server.MapPath(virtualPath);

        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool FallbackOnError { get; set; }
        public bool BundleFiles { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public bool ETag { get; set; }
    }
}
