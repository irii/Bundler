using System;

namespace Bundler.Infrastructure {
    public interface IBundle : IDisposable {
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
        /// Adds a new content source to the Bundle
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ISource source);

        /// <summary>
        /// Refreshes the bundle.
        /// </summary>
        bool Refresh();

        /// <summary>
        /// Handler callback for custom dependencies.
        /// </summary>
        SourceChangedDelegate ChangeHandler { get; }

        /// <summary>
        /// Renders a bundle specific tag.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string Render(string url);
    }
}