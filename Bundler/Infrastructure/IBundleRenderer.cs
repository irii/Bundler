using System.Collections.Generic;

namespace Bundler.Infrastructure {

    /// <summary>
    /// Provides render methods and processing meta informations
    /// </summary>
    public interface IBundleRenderer {
        /// <summary>
        /// Renders a specific content tag based on the used implementation
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string Render(string url);

        /// <summary>
        /// Response Content Type
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Concats single items to one
        /// </summary>
        /// <param name="processedItems"></param>
        /// <returns></returns>
        string Concat(IEnumerable<string> processedItems);
    }
}
