using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Bundler.Infrastructure;
using Bundler.Internals;

namespace Bundler {
    public static class Bundler {
        public static string Content(string bundleKey) {
            BundleInfo bundleInfo;
            if (!BundleInfoStore.GetBundleInfoByKey(bundleKey, out bundleInfo)) {
                return string.Empty;
            }

            return bundleInfo.Container.Get();
        }

        public static bool IsBundleRequest(Uri uri, out BundleInfo bundleInfo) {
            return BundleInfoStore.GetBundleInfoByPath(uri.AbsolutePath, out bundleInfo);
        }

        public static void RegisterContentBundler(string bundleKey, IContentBundler contentBundler) {
            var path = $"/{Guid.NewGuid():N}";
            RegisterContentBundler(bundleKey, path, contentBundler);
        }

        public static void RegisterContentBundler(string bundleKey, string path, IContentBundler contentBundler) {
            BundleInfoStore.RegisterKey(bundleKey, path, contentBundler);
        }

        public static bool IsBundleKeyRegistered(string bundleKey) {
            return BundleInfoStore.IsBundleKeyRegistered(bundleKey);
        }

        public static void RegisterFile(string bundleKey, string virtualFile) {
            BundleInfo bundleInfo;
            if (!BundleInfoStore.GetBundleInfoByKey(bundleKey, out bundleInfo) || bundleInfo.Container.Exists(virtualFile)) {
                return;
            }

            if (!VirtualPathFileHelper.Exists(virtualFile)) {
                return;
            }

            var fileContent = File.ReadAllText(VirtualPathFileHelper.GetFilePath(virtualFile));

            using (var transformer = bundleInfo.ContentBundler.CreateTransformer()) {
                string processedContent;
                if (!transformer.Process(fileContent, out processedContent)) {
                    return;
                }

                bundleInfo.Container.Append(virtualFile, processedContent, bundleInfo.ContentBundler.Placeholder);
            }
        }

        public static IHtmlString RenderTag(string bundleKey) {
            BundleInfo bundleInfo;
            if (!BundleInfoStore.GetBundleInfoByKey(bundleKey, out bundleInfo)) {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(Environment.NewLine + bundleInfo.ContentBundler.GenerateTag(bundleInfo.VirtualPath + "?v=" + bundleInfo.Container.GetVersion()));
        }
    }
}
