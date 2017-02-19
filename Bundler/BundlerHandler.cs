using System.Web;
using Bundler.Infrastructure;

namespace Bundler {
    public sealed class BundlerHandler : IHttpHandler {
        private readonly BundleInfo _bundleInfo;
        public bool IsReusable { get; } = false;

        public BundlerHandler(BundleInfo bundleInfo) {
            _bundleInfo = bundleInfo;
        }


        public void ProcessRequest(HttpContext context) {
            context.Response.Write(Bundler.Content(_bundleInfo.BundleKey) ?? string.Empty);
            context.Response.ContentType = _bundleInfo.ContentBundler.ContentType;
        }

    }
}