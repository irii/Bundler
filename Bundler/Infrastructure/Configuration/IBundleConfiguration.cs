namespace Bundler.Infrastructure.Configuration {
    public interface IBundleConfiguration {
        /// <summary>
        /// Get setting class
        /// </summary>
        /// <typeparam name="T">Setting class</typeparam>
        /// <returns></returns>
        T Get<T>(Setting<T> key);
    }
}
