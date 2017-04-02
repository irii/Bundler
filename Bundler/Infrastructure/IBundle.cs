using System;

namespace Bundler.Infrastructure {
    public interface IBundle : IDisposable {
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
        /// <param name="source"></param>
        /// <returns></returns>
        bool Include(ISource source);

        /// <summary>
        /// Refreshes the bundle.
        /// </summary>
        bool Refresh();

        /// <summary>
        /// Handler callback for custom dependencies.
        /// </summary>
        FileChangedDelegate ChangeHandler { get; }
    }
}