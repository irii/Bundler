using System;

namespace Bundler.Infrastructure {
    public interface IBundleConfiguration {
        /// <summary>
        /// Returns the query parameter name of the version argument.
        /// </summary>
        string VersionQueryParameterName { get; }

        /// <summary>
        /// Returns the query parameter name of the file argument.
        /// </summary>
        string FileQueryParameterName { get; }

        /// <summary>
        /// Get Property value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        T GetValue<T>(Property<T> property);

        /// <summary>
        /// Optimization
        /// </summary>
        bool Optimization { get; }

        /// <summary>
        /// Enable Server cache
        /// </summary>
        bool Cache { get; }

        /// <summary>
        /// Append ETag
        /// </summary>
        bool ETag { get; }

        /// <summary>
        /// Server cache duration
        /// </summary>
        TimeSpan CacheDuration { get; }

        /// <summary>
        /// Use input content if processing has failed
        /// </summary>
        bool FallbackOnError { get; }

        /// <summary>
        /// Combine all files to one.
        /// </summary>
        bool BundleFiles { get; }

        /// <summary>
        /// Refresh bundle on source change.
        /// </summary>
        bool AutoRefresh { get; }

    }

    public struct Property<T> {
        public Property(Guid identifier, string name) : this(identifier, name, default(T)) { }

        public Property(Guid identifier, string name, T @default) {
            Identifier = identifier;
            Name = name;
            Default = @default;
        }
        public Guid Identifier { get; }

        public string Name { get; }

        public T Default { get; }
    }
}
