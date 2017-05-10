namespace Bundler.Infrastructure.Configuration {
    public struct Setting<T> {
        public Setting(string section, string name, T defaultValue = default(T)) {
            Key = $"{section}::{name}";
            Default = defaultValue;
        }

        public string Key { get; }
        public T Default { get; }
    }
}