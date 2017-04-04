using System;
using Bundler.Infrastructure;

namespace Bundler.Helper {
    public static class ConfigurationValidator {
        public static void Validate(IBundleContext bundleContext) {
            if (bundleContext == null) throw new ArgumentNullException(nameof(bundleContext));
            if (bundleContext.Watcher == null) throw new NullReferenceException($"{nameof(IBundleContext)}.{nameof(IBundleContext.Watcher)}");
            if (bundleContext.UrlHelper == null) throw new NullReferenceException($"{nameof(IBundleContext)}.{nameof(IBundleContext.UrlHelper)}");
            if (bundleContext.VirtualPathProvider == null) throw new NullReferenceException($"{nameof(IBundleContext)}.{nameof(IBundleContext.VirtualPathProvider)}");
            if (bundleContext.Configuration == null) throw new NullReferenceException($"{nameof(IBundleContext)}.{nameof(IBundleContext.Configuration)}");

            if (string.IsNullOrWhiteSpace(bundleContext.Configuration.FileQueryParameterName)) throw new Exception($"{nameof(IBundleConfiguration)}.{nameof(IBundleConfiguration.FileQueryParameterName)} is null or whitespace.");
            if (string.IsNullOrWhiteSpace(bundleContext.Configuration.VersionQueryParameterName)) throw new Exception($"{nameof(IBundleConfiguration)}.{nameof(IBundleConfiguration.VersionQueryParameterName)} is null or whitespace.");
            if (string.Equals(bundleContext.Configuration.FileQueryParameterName, bundleContext.Configuration.VersionQueryParameterName, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"{nameof(IBundleConfiguration)}.{nameof(IBundleConfiguration.FileQueryParameterName)} and {nameof(IBundleConfiguration)}.{nameof(IBundleConfiguration.VersionQueryParameterName)} cannot be equal.");
        }
    }
}
