using System;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Configuration;

namespace Bundler.Less {
    public static class LessConfiguration {
        public const string Section = "Less";
        public static readonly Setting<bool> Compress = new Setting<bool>(Section, "Compress");
        public static readonly Setting<bool> Debug = new Setting<bool>(Section, "Debug");

        public static IBundleConfigurationBuilder SetupLess(this IBundleConfigurationBuilder bundleConfigurationBuilder, LessSettings settings) {
            if (bundleConfigurationBuilder == null) throw new ArgumentNullException(nameof(bundleConfigurationBuilder));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            bundleConfigurationBuilder.Set(Compress, settings.Compress);
            bundleConfigurationBuilder.Set(Debug, settings.Debug);

            return bundleConfigurationBuilder;
        }
    }

    public sealed class LessSettings {
        public bool Compress { get; set; } = LessConfiguration.Compress.Default;
        public bool Debug { get; set; } = LessConfiguration.Debug.Default;
    }
}
