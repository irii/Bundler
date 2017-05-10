using System.Collections.Generic;
using Bundler.Infrastructure.Server;

namespace Bundler.Infrastructure {
    public interface ISource {
        /// <summary>
        /// Returns a hashcode or a unique string which represents the source configuration.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Adds a list of all items which should be added to the bundle.
        /// </summary>
        /// <param name="bundleContext"></param>
        /// <param name="items"></param>
        /// <param name="watchPaths">List of paths which should be used for the file system watcher</param>
        /// <returns></returns>
        bool AddItems(IBundleContext bundleContext, ICollection<ISourceItem> items, ICollection<string> watchPaths);
    }
}
