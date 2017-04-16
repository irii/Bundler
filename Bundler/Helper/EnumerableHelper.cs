using System.Collections.Generic;
using System.Linq;

namespace Bundler.Helper {
    /// <summary>
    /// Useful helpers for IEnumerable'1
    /// </summary>
    public static class EnumerableHelper {
        public static IEnumerable<T> Union<T>(this IEnumerable<T> first, params T[] with) {
            return Enumerable.Union(first, with);
        }
    }
}
