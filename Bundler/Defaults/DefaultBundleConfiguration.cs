using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    public class DefaultBundleConfiguration : IBundleConfiguration {
        private readonly IDictionary<Guid, object> _properties = new Dictionary<Guid, object>();

        public string VersionQueryParameterName { get; } = "v";
        public string FileQueryParameterName { get; } = "f";

        public bool Optimization { get; set; }
        public bool Cache { get; set; }
        public bool ETag { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public bool FallbackOnError { get; set; }
        public bool BundleFiles { get; set; }
        public bool AutoRefresh { get; set; }
        
        public T GetValue<T>(Property<T> property) {
            object value;
            if (_properties.TryGetValue(property.Identifier, out value) && value != null) {
                return (T) value;
            }

            return property.Default;
        }

        public DefaultBundleConfiguration SetValue<T>(Property<T> property, T value) {
            _properties[property.Identifier] = value;
            return this;
        }
    }
}
