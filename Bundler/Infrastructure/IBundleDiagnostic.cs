using System;

namespace Bundler.Infrastructure {
    public interface IBundleDiagnostic {
        void Log(LogLevel logLevel, string source, string method, string message, Exception exception = null);
    }

    public enum LogLevel {
        None = 0,
        Debug = 1,
        Info = 2,
        Warn = 4,
        Error = 8,
        Critical = 16
    }
}
