using System;

namespace Bundler.Infrastructure {
    public interface IBundleProvider {
        IBundleContext Context { get; }

        bool Add(string virtualPath, IBundle bundle);

        bool Get(string virtualPath, out IBundle bundle);
        bool Exists(string virtualPath);

        string Render(string virtualPath);

        bool ResolveUri(Uri uri, out IBundle bundle, out int requestVersion, out string requestFile);
    }
}
