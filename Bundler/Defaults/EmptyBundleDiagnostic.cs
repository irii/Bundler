using System;
using Bundler.Infrastructure;
using Bundler.Infrastructure.Server;

namespace Bundler.Defaults {
    public sealed class EmptyBundleDiagnostic : IBundleDiagnostic {
        public void Log(LogLevel logLevel, string source, string method, string message, Exception exception = null) { }
    }
}
