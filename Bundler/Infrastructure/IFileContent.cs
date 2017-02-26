namespace Bundler.Infrastructure {
    public interface IFileContent {
        /// <summary>
        /// Virtual file
        /// </summary>
        string VirtualFile { get; }

        /// <summary>
        /// Transformed content (Shared between ContentTransformers)
        /// </summary>
        string Content { get; set; }
    }
}
