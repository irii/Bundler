using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler.Comparers {
    public class SourceItemEqualityComparer : IEqualityComparer<ISourceItem> {
        public static readonly IEqualityComparer<ISourceItem> Default = new SourceItemEqualityComparer();

        public bool Equals(ISourceItem x, ISourceItem y) {
            return string.Equals(x.VirtualFile, y.VirtualFile, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ISourceItem obj) {
            return obj.VirtualFile.GetHashCode();
        }
    }
}