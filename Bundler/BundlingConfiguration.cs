using System;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Configuration;

namespace Bundler {
    public static class BundlingConfiguration {
        public const string Section = "Bundling";

        public static readonly Setting<bool> CombineResponse = new Setting<bool>(Section, "CombineResponse", true);
        public static readonly Setting<bool> AutoRefresh = new Setting<bool>(Section, "AutoRefresh", true);
        public static readonly Setting<bool> FallbackOnError = new Setting<bool>(Section, "FallbackOnError", true);
        public static readonly Setting<bool> IncludeContentHash = new Setting<bool>(Section, "IncludeContentHash", true);

        public static IBundleConfigurationBuilder SetupBundling(this IBundleConfigurationBuilder bundleConfigurationBuilder, BundlingSettings settings) {
            if (bundleConfigurationBuilder == null) throw new ArgumentNullException(nameof(bundleConfigurationBuilder));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            bundleConfigurationBuilder.Set(CombineResponse, settings.CombineResponse);
            bundleConfigurationBuilder.Set(FallbackOnError, settings.FallbackOnError);
            bundleConfigurationBuilder.Set(AutoRefresh, settings.AutoRefresh);
            bundleConfigurationBuilder.Set(IncludeContentHash, settings.IncludeContentHash);

            return bundleConfigurationBuilder;
        }
    }

    public sealed class BundlingSettings {
        /// <summary>
        /// Combine multiple files
        /// </summary>
        public bool CombineResponse { get; set; } = BundlingConfiguration.CombineResponse.Default;

        /// <summary>
        /// Use input source if processing has failed
        /// </summary>
        public bool FallbackOnError { get; set; } = BundlingConfiguration.FallbackOnError.Default;

        /// <summary>
        /// Refresh bundles on file change
        /// </summary>
        public bool AutoRefresh { get; set; } = BundlingConfiguration.AutoRefresh.Default;

        /// <summary>
        /// Append Content hash to query string
        /// </summary>
        public bool IncludeContentHash { get; set; } = BundlingConfiguration.IncludeContentHash.Default;
    }
}
