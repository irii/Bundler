using System;
using System.Collections.Generic;
using Bundler.Infrastructure;

namespace Bundler.Comparers {
    public class SourceEqualityComparer : IEqualityComparer<ISource> {
        public static readonly IEqualityComparer<ISource> Default = new SourceEqualityComparer();

        public bool Equals(ISource x, ISource y) {
            return string.Equals(x.Identifier, y.Identifier, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ISource obj) {
            return obj.Identifier.GetHashCode();
        }
    }
}