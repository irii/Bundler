using System;

namespace Bundler.Infrastructure {
    public interface ISourceItem : IDisposable {
        /// <summary>
        /// Virtual path of the item.
        /// </summary>
        string VirtualFile { get; }

        /// <summary>
        /// Returns the content of the item.
        /// </summary>
        /// <returns></returns>
        string Get();
    }
}