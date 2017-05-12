using System.IO.Compression;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bundler.AspNet;
using Bundler.Defaults;
using Bundler.Example;
using Bundler.Infrastructure;
using Bundler.JavaScript;
using Bundler.Less;

[assembly: PreApplicationStartMethod(typeof(MvcApplication), nameof(MvcApplication.InitBundlerModule))]

namespace Bundler.Example {
    public class MvcApplication : System.Web.HttpApplication {
        public IBundleProvider BundleProvider => AspNetBundler.Current;

        public static void InitBundlerModule() {
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(AspNetBundler.HttpModule);
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var configuration = new DefaultBundleConfigurationBuilder()
                .SetupBundling(new BundlingSettings {
                    AutoRefresh = true,
                    CombineResponse = true,
                    FallbackOnError = true,
                    IncludeContentHash = true
                })
                .SetupCaching(new CachingSettings {
                    Enabled = true,
                    UseEtag = true
                })
                .SetupCompression(new CompressSettings {
                    CompressionAlgorithm = CompressionAlgorithm.All
                })
                .SetupLess(new LessSettings {
                    Compress = true,
                    Debug = true
                })
                .SetupJavaScript(new JavaScriptSettings {
                    Minify = true
                })
                .Create();

            // Setup BundleProvider
            var bundleContext = new AspNetBundleContext(configuration, new DebugBundleDiagnostic());
            var bundleProvider = new BundleProvider(bundleContext);

            // Init initial bundles
            BundleConfig.SetupBundler(bundleProvider);

            // Initialize Bundling engine
            AspNetBundler.Initialize(bundleProvider);
        }
    }
}
