using System;

namespace Bundler.Infrastructure {
    public interface IBundleProvider {
        IBundleProviderConfiguration Configuration { get; }

        bool Add(string virtualPath, IContentBundler contentBundler, out IBundle bundle);
        bool AddOrGet(string virtualPath, Func<IContentBundler> contentBundlerFactory, out IBundle bundle);

        bool Get(string virtualPath, out IBundle bundle);
        bool Exists(string virtualPath);
    }
}
