using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleProvider {
        /// <summary>
        /// Content
        /// </summary>
        IBundleContext Context { get; }

        /// <summary>
        /// Returns a list of all registered bundles
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IBundle> GetBundles();

        /// <summary>
        /// Add's a bundle to the collection. Throws a exception if bundle already exists.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        bool Add(string virtualPath, IBundle bundle);

        /// <summary>
        /// Returns the bundle object if exists.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        bool Get(string virtualPath, out IBundle bundle);

        /// <summary>
        /// Checks if a bundle is registered to the virtual path.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool Exists(string virtualPath);

        /// <summary>
        /// Renders a html tag based on the tagformat.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string Render(string virtualPath);

        /// <summary>
        /// Tries to resolve the request url.
        /// Returns a bundle if matching was success.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="bundle"></param>
        /// <param name="requestFile"></param>
        /// <returns></returns>
        bool ResolveUri(Uri uri, out IBundle bundle, out string requestFile);
    }
}
