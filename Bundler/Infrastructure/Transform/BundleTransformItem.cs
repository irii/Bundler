using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure.Transform {
    public class BundleTransformItem {
        private bool _canUseFallback = true;

        public BundleTransformItem(string virtualFile, string inputContent) {
            VirtualFile = virtualFile;
            Content = inputContent;
        }

        /// <summary>
        /// Virtual File Path
        /// </summary>
        public string VirtualFile { get; }

        /// <summary>
        /// List of all occurred errors
        /// </summary>
        public ICollection<string> Errors { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Transformed content
        /// </summary>

        public string Content { get; set; }

        /// <summary>
        /// Dependent paths for the virtual file.
        /// </summary>
        public ICollection<string> WatchPaths { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Determines if the bundling engine can use the input content for fallback handling.
        /// This can vary by different content types.
        /// </summary>
        public bool CanUseFallback {
            get { return _canUseFallback; }
            set { _canUseFallback &= value; }
        }
    }
}