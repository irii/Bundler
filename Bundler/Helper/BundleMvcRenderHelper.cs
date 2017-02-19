using System;
using System.Web.Mvc;

namespace Bundler.Helper {
    public static class BundleMvcRenderHelper {
        public static MvcHtmlString RenderMvc(this Bundle bundle) {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            return new MvcHtmlString(Environment.NewLine + bundle.Render());
        }
    }
}
