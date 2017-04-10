using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Bundler.Infrastructure;

namespace Bundler.AspNet {
    public class AspNetBundleVirtualPathProvider : IBundleVirtualPathProvider {
        public Stream Open(string virtualPath) {
            var virtualFile = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);
            if (virtualFile == null) {
                throw new FileNotFoundException("Can't find file", virtualPath);
            }

            return virtualFile.Open();
        }

        public bool FileExists(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
        }

        public bool DirectoryExists(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualPath);
        }

        public IEnumerable<string> EnumerateFiles(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath)
                .Files.OfType<VirtualFileBase>()
                .Select(x => x.VirtualPath)
                .ToList();
        }

        public IEnumerable<string> EnumerateDirectories(string virtualPath) {
            return HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath)
                .Directories.OfType<VirtualFileBase>()
                .Select(x => x.VirtualPath)
                .ToList();
        }

        public void Dispose() {}

        public string GetPhysicalPath(string virtualPath) {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public string GetVirtualPath(string absolutePath) {
            if (absolutePath == null) throw new ArgumentNullException(nameof(absolutePath));
            absolutePath = Path.GetFullPath(absolutePath);
            if (!File.Exists(absolutePath) && !Directory.Exists(absolutePath)) {
                throw new Exception($"File or Directory not found! {absolutePath}");
            }

            var root = new Uri(GetPhysicalPath("~/"), UriKind.Absolute);
            var requestedPath = new Uri(absolutePath, UriKind.Absolute);

            if (!requestedPath.AbsolutePath.StartsWith(root.AbsolutePath, StringComparison.InvariantCultureIgnoreCase)) {
                throw new Exception("Can't resolve app dir.");
            }

            var basePath = requestedPath.AbsolutePath.Substring(root.AbsolutePath.Length);
            
            var virtualPath = ("~/" + basePath).Replace("\\", "/");
            return virtualPath;
        }
    }
}