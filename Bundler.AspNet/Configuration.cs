using System;
using System.IO.Compression;
using Bundler.Infrastructure.Configuration;

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


    public static class CompressionConfiguration {
        public const string Section = "Asp_Bundle_Compression";
     
        public static Setting<CompressionAlgorithm> CompressionAlgorithm { get; } = new Setting<CompressionAlgorithm>(Section, "CompressionAlgorithm", AspNet.CompressionAlgorithm.None);
        public static Setting<CompressionLevel> CompressionLevel { get; } = new Setting<CompressionLevel>(Section, "CompressionLevel", System.IO.Compression.CompressionLevel.Optimal);

        public static IBundleConfigurationBuilder SetupCompression(this IBundleConfigurationBuilder bundleConfigurationBuilder, CompressSettings settings) {
            bundleConfigurationBuilder.Set(CompressionAlgorithm, settings.CompressionAlgorithm);
            bundleConfigurationBuilder.Set(CompressionLevel, settings.CompressionLevel);
            return bundleConfigurationBuilder;
        }
    }

    [Flags]
    public enum CompressionAlgorithm {
        None = 0,
        Gzip = 1,
        Deflate = 2,
        All = ~None
    }

    public sealed class CompressSettings {
        public CompressionAlgorithm CompressionAlgorithm { get; set; } = CompressionConfiguration.CompressionAlgorithm.Default;
        public CompressionLevel CompressionLevel { get; set; } = CompressionConfiguration.CompressionLevel.Default;
    }
}
