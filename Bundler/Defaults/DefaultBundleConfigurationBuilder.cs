using System.Collections.Generic;
using System.Linq;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    public class DefaultBundleConfigurationBuilder : IBundleConfigurationBuilder {
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        public IBundleConfiguration Create() => new DefaultBundleConfiguration(_properties.ToDictionary(x => x.Key, y => y.Value));

        public void Set<T>(Setting<T> key, T value) {
            _properties[key.Key] = value;
        }

        public T Get<T>(Setting<T> key) {
            object value;
            if (_properties.TryGetValue(key.Key, out value)) {
                return (T)value;
            }

            return key.Default;
        }
    }
}