using Bundler.Helper;
using Bundler.Infrastructure;

namespace Bundler {
    public class DefaultBundleFileWatcher : IBundleFileWatcher {
        private readonly IBundleVirtualPathProvider _virtualPathProvider;
        private readonly FileWatcher _fileWatcher;


        public DefaultBundleFileWatcher(IBundleVirtualPathProvider virtualPathProvider) {
            _virtualPathProvider = virtualPathProvider;
            _fileWatcher = new FileWatcher(virtualPathProvider, "~/");

            _fileWatcher.StartWatching();
        }

        public void Watch(string virtualPath, FileChangedDelegate callback) {
            _fileWatcher.Watch(virtualPath, callback);
        }

        public void Unwatch(string virtualPath, FileChangedDelegate callback) {
            _fileWatcher.Unwatch(virtualPath, callback);
        }

        public void Dispose() {
            _fileWatcher.Dispose();
        }
    }
}
