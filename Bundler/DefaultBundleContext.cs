using System;
using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public class DefaultBundleContext : IBundleContext {
        private readonly HttpApplication _httpApplication;
        public DefaultBundleContext() : this(HttpContext.Current.ApplicationInstance) { }

        public DefaultBundleContext(HttpApplication httpApplication) {
            _httpApplication = httpApplication;
        }

        public string GetFullPath(string virtualPath) => _httpApplication.Server.MapPath(virtualPath);

        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool FallbackOnError { get; set; }
        public TimeSpan CacheDuration { get; set; }
    }
}
