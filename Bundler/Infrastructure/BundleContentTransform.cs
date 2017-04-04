using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public class BundleContentTransform {

        public BundleContentTransform(string virtualPath, string inputContent) {
            VirtualPath = virtualPath;
            Content = inputContent;
        }

        public string VirtualPath { get; }

        public ICollection<string> Errors { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public string Content { get; set; }
    }
}