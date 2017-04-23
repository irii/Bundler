namespace Bundler.Infrastructure {
    public interface IBundleConfiguration {
        /// <summary>
        /// Get setting class
        /// </summary>
        /// <typeparam name="T">Setting class</typeparam>
        /// <returns></returns>
        T Get<T>(Setting<T> key);
    }

    public interface IBundleConfigurationBuilder {
        /// <summary>
        /// Create Configuration
        /// </summary>
        /// <returns></returns>
        IBundleConfiguration Create();

        /// <summary>
        /// Set setting class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void Set<T>(Setting<T> key, T value);

        /// <summary>
        /// Get setting class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(Setting<T> key);
    }

    public struct Setting<T> {
        public Setting(string section, string name, T defaultValue = default(T)) {
            Key = $"{section}::{name}";
            Default = defaultValue;
        }

        public string Key { get; }
        public T Default { get; }
    }
}
