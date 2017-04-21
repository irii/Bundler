using System;
using System.Collections.Generic;
using System.Linq;
using Bundler.Comparers;
using Bundler.Infrastructure;
using Bundler.Sources;

namespace Bundler {
    public sealed class BundleBuilder {
        private readonly IBundleContext _bundleContext;
        private readonly string _contentType;
        private readonly string _placeHolder;
        private readonly string _tagFormat;

        public BundleBuilder(IBundleContext bundleContext, string contentType, string placeHolder, string tagFormat) {
            _bundleContext = bundleContext;
            _contentType = contentType;
            _placeHolder = placeHolder;
            _tagFormat = tagFormat;
        }

        public ICollection<ISource> Sources { get; } = new HashSet<ISource>(SourceEqualityComparer.Default);
        public ICollection<IBundleContentTransformer> ContentTransformers { get; } = new HashSet<IBundleContentTransformer>();

        public BundleBuilder AddSource(ISource source) {
            Sources.Add(source);
            return this;
        }
        
        public BundleBuilder AddContentTransformer(IBundleContentTransformer bundleContentTransformer) {
            ContentTransformers.Add(bundleContentTransformer);
            return this;
        }

        public IBundle Create() {
            var bundle = new Bundle(_bundleContext, _contentType, _placeHolder, _tagFormat, ContentTransformers.ToArray());
            if (!bundle.Add(Sources.ToArray())) {
                throw new Exception($"Failed to construct bundle with this sources ${string.Join("; ", Sources.Select(x => x.Identifier))}");
            }

            return bundle;
        }
    }

    public static class BundleBuilderHelper {
        public static BundleBuilder AddFile(this BundleBuilder bundleBuilder, string virtualFile) {
            return bundleBuilder.AddSource(new StreamSource(virtualFile));
        }
    }
}
