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
        /// Adds new content sources to the Bundle
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        bool Add(params ISource[] sources);

        /// <summary>
        /// Refreshes the bundle.
        /// </summary>
        bool Refresh();

        /// <summary>
        /// Renders a bundle specific tag.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string Render(string url);
    }
}