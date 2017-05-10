using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

namespace Bundler.Sources {
    public class DirectorySource : ISource {
        private readonly Regex _searchPattern;
        public bool IsWatchable { get; } = true;

        public DirectorySource(string virtualPath, string regexSearchPattern, bool includeSubDirectories) {
            VirtualPath = virtualPath;
            Identifier = virtualPath;

            _searchPattern = new Regex(regexSearchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            IncludeSubDirectories = includeSubDirectories;
        }

        public string Identifier { get; }

        public string VirtualPath { get; }
        public bool IncludeSubDirectories { get; }

        public bool AddItems(IBundleContext bundleContext, ICollection<ISourceItem> items, ICollection<string> watchPaths) {
            if (!AddFiles(bundleContext, VirtualPath, items, watchPaths)) {
                return false;
            }

            watchPaths.Add(VirtualPath);

            var success = AddFiles(bundleContext, VirtualPath, items, watchPaths);
            return success;
        }

        private bool AddFiles(IBundleContext bundleContext, string folder, ICollection<ISourceItem> items, ICollection<string> watchPaths) {
            watchPaths.Add(folder);

            foreach (var file in bundleContext.VirtualPathProvider.EnumerateFiles(folder)) {
                if (!_searchPattern.IsMatch(file)) {
                    continue;
                }

                if (!new StreamSource(file).AddItems(bundleContext, items, watchPaths)) {
                    return false;
                }

                foreach (var childDirectory in bundleContext.VirtualPathProvider.EnumerateDirectories(folder)) {
                    AddFiles(bundleContext, childDirectory, items, watchPaths);
                }
            }

            return true;
        }

    }
}
