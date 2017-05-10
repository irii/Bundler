using System;

namespace Bundler.Infrastructure.Server {
    public interface IBundleDiagnostic {
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="source"></param>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
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
