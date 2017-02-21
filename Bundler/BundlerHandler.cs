using System.Web;

namespace Bundler {
    public sealed class BundlerHandler : IHttpHandler {
        private readonly Bundle _bundle;
        public bool IsReusable { get; } = false;

        public BundlerHandler(Bundle bundle) {
            _bundle = bundle;
        }


        public void ProcessRequest(HttpContext context) {
            context.Response.Write(_bundle.Content);
            context.Response.ContentType = _bundle.ContentType;
        }

    }
}