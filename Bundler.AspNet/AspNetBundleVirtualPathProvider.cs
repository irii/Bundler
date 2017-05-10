using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

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
    }
}