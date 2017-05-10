using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Configuration;

namespace Bundler.Defaults {
    public class DefaultBundleConfiguration : IBundleConfiguration {
        private readonly IReadOnlyDictionary<string, object> _settings;

        public DefaultBundleConfiguration(IDictionary<string, object> properties) {
            _settings = new ReadOnlyDictionary<string, object>(properties);
        }

        public T Get<T>(Setting<T> key) {
            object value;
            if (_settings.TryGetValue(key.Key, out value)) {
                return (T)value;
            }

            return key.Default;
        }
    }
}
