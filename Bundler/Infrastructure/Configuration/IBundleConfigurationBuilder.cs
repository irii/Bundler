namespace Bundler.Infrastructure.Configuration {
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
}