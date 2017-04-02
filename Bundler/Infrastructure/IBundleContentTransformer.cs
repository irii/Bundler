using System;
using System.Collections.Generic;

namespace Bundler.Infrastructure {
    public interface IBundleContentTransformer : IDisposable {
        bool Process(IBundle bundle, IBundleContentTransformResult bundleContentTransformResult);
    }

    public interface IBundleContentTransformResult {
        string VirtualPath { get; }

        ICollection<string> Errors { get; }

        string Content { get; set; }
    }

    public interface IBundleResponseTransformer : IDisposable {
        bool Process(IBundleContext bundleContext, IBundleResponse bundleResponse);
    }
}