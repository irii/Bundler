using System.IO;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;
using dotless.Core.Input;

namespace Bundler.Less {
    public class DotLessVirtualFileReader : IFileReader {
        private readonly IBundleVirtualPathProvider _virtualPathProvider;

        public DotLessVirtualFileReader(IBundleVirtualPathProvider virtualPathProvider) {
            _virtualPathProvider = virtualPathProvider;
        }

        public bool UseCacheDependencies { get; } = false;

        public byte[] GetBinaryFileContents(string fileName) {
            using (var stream = _virtualPathProvider.Open(fileName)) {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        public string GetFileContents(string fileName) {
            using (var stream = _virtualPathProvider.Open(fileName)) {
                using (var streamReader = new StreamReader(stream)) {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public bool DoesFileExist(string fileName) => _virtualPathProvider.FileExists(fileName);
    }
}