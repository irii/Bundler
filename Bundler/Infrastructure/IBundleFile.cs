namespace Bundler.Infrastructure {
    public interface IBundleFile {
        /// <summary>
        /// Virtual file
        /// </summary>
        string VirtualFile { get; }

        /// <summary>
        /// Version
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Transformed content
        /// </summary>
        string Content { get; }
    }
}