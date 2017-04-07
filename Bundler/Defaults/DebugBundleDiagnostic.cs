using System;
using System.Diagnostics;
using Bundler.Infrastructure;

namespace Bundler.Defaults {
    public sealed class DebugBundleDiagnostic : IBundleDiagnostic {
        private readonly LogLevel _requiredLogLevel = LogLevel.Debug;
        public DebugBundleDiagnostic() {}

        public DebugBundleDiagnostic(LogLevel requiredLogLevel) {
            _requiredLogLevel = requiredLogLevel;
        }

        public void Log(LogLevel logLevel, string source, string method, string message, Exception exception = null) {
            if (logLevel < _requiredLogLevel) {
                return;
            }

            if (exception != null) {
                message += " " + exception.Message;
            }

            Debug.WriteLine($"[{logLevel}, {source}->{method}] {message}");
        }
    }
}
