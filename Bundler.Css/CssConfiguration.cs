using System;
using Bundler.Infrastructure;

namespace Bundler.Css {
    public static class CssConfiguration {
        public const string Section = "CSS";
        public static readonly Setting<bool> Minify = new Setting<bool>(Section, "Minify");

        public static IBundleConfigurationBuilder SetupCss(this IBundleConfigurationBuilder bundleConfigurationBuilder, CssSettings cssSettings) {
            if (bundleConfigurationBuilder == null) throw new ArgumentNullException(nameof(bundleConfigurationBuilder));
            if (cssSettings == null) throw new ArgumentNullException(nameof(cssSettings));

            bundleConfigurationBuilder.Set(Minify, cssSettings.Minify);
            return bundleConfigurationBuilder;
        }
    }

    public sealed class CssSettings {
        public bool Minify { get; set; } = CssConfiguration.Minify.Default;
    }
}