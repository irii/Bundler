namespace Bundler.Infrastructure {
    public interface IBundleFileResponse : IBundleContentResponse {
        /// <summary>
        /// Filename
        /// </summary>
        string VirtualFile { get; }
    }
}