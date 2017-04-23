using System;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public static class CachingConfiguration {
        public const string Section = "Asp_Bundle_Caching";

        public static Setting<bool> Enabled { get; } = new Setting<bool>(Section, "Enable", false);
        public static Setting<bool> UseEtag { get; } = new Setting<bool>(Section, "Use_ETag", false);
        public static Setting<TimeSpan> Duration { get; } = new Setting<TimeSpan>(Section, "Duration", new TimeSpan(30, 0, 0, 0));

        public static IBundleConfigurationBuilder SetupCaching(this IBundleConfigurationBuilder bundleConfigurationBuilder, CachingSettings settings) {
            if (bundleConfigurationBuilder == null) throw new ArgumentNullException(nameof(bundleConfigurationBuilder));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            bundleConfigurationBuilder.Set(Enabled, settings.Enabled);
            bundleConfigurationBuilder.Set(UseEtag, settings.UseEtag);
            bundleConfigurationBuilder.Set(Duration, settings.Duration);

            return bundleConfigurationBuilder;
        }
    }

    public sealed class CachingSettings {
        public bool Enabled { get; set; } = CachingConfiguration.Enabled.Default;

        public bool UseEtag { get; set; } = CachingConfiguration.UseEtag.Default;

        public TimeSpan Duration { get; set; } = CachingConfiguration.Duration.Default;
    }
}
