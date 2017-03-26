using System;

namespace Bundler.Infrastructure {
    public delegate void SourceChangedEvent(object sender, EventArgs args);

    public interface IContentSource : IDisposable {
        /// <summary>
        /// Virtual contentSource
        /// </summary>
        string VirtualFile { get; }

        /// <summary>
        /// Returns the actual contentSource content
        /// </summary>
        /// <returns></returns>
        string Get();

        /// <summary>
        /// Get's fired when the source is changed
        /// </summary>
        event SourceChangedEvent OnSourceChanged;
    }
}
