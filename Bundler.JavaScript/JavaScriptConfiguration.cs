using System;
using Bundler.Infrastructure;

namespace Bundler.JavaScript {
    public static class JavaScriptConfiguration {
        public const string Section = "JavaScript";
        public static readonly Setting<bool> Minify = new Setting<bool>(Section, "Minify");

        public static IBundleConfigurationBuilder SetupJavaScript(this IBundleConfigurationBuilder bundleConfigurationBuilder, JavaScriptSettings javaScriptSettings) {
            if (bundleConfigurationBuilder == null) throw new ArgumentNullException(nameof(bundleConfigurationBuilder));
            if (javaScriptSettings == null) throw new ArgumentNullException(nameof(javaScriptSettings));

            bundleConfigurationBuilder.Set(Minify, javaScriptSettings.Minify);
            return bundleConfigurationBuilder;
        }
    }

    public sealed class JavaScriptSettings {
        public bool Minify { get; set; } = JavaScriptConfiguration.Minify.Default;
    }
}