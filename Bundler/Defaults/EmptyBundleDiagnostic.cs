using System;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    public sealed class EmptyBundleDiagnostic : IBundleDiagnostic {
        public void Log(LogLevel logLevel, string source, string method, string message, Exception exception = null) { }
    }
}
