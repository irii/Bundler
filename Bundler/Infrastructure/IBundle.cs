namespace Bundler.Infrastructure {
    public interface IBundle {
        /// <summary>
        /// Used format for rendering
        /// </summary>
        string TagFormat { get; }

        /// <summary>
        /// Context
        /// </summary>
        IBundleContext Context { get; }

        /// <summary>
        /// Returns a response object for the current bundle.
        /// </summary>
        /// <returns></returns>
        IBundleResponse GetResponse();

        /// <summary>
        /// Includes a new file to the Bundle
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool Include(string virtualFile, string content);
    }
}