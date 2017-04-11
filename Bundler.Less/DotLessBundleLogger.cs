using Bundler.Infrastructure;
using dotless.Core.Loggers;
using LogLevel = dotless.Core.Loggers.LogLevel;

namespace Bundler.Less {
    public class DotLessBundleLogger : ILogger {
        private readonly IBundleDiagnostic _bundleDiagnostic;

        public DotLessBundleLogger(IBundleDiagnostic bundleDiagnostic) {
            _bundleDiagnostic = bundleDiagnostic;
        }

        public void Log(LogLevel level, string message) {
            Infrastructure.LogLevel bundleLogLevel;

            switch (level) {
                case LogLevel.Info:
                    bundleLogLevel = Infrastructure.LogLevel.Info;
                    break;
                case LogLevel.Debug:
                    bundleLogLevel = Infrastructure.LogLevel.Debug;
                    break;
                case LogLevel.Warn:
                    bundleLogLevel = Infrastructure.LogLevel.Warn;
                    break;
                case LogLevel.Error:
                    bundleLogLevel = Infrastructure.LogLevel.Error;
                    break;
                default:
                    bundleLogLevel = Infrastructure.LogLevel.Info;
                    break;
            }

            _bundleDiagnostic.Log(bundleLogLevel, nameof(LessBundleContentTransformer), nameof(IBundleContentTransformer.Process), message);
        }

        public void Info(string message) => Log(LogLevel.Info, message);
        public void Info(string message, params object[] args) => Info(string.Format(message, args));

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Debug(string message, params object[] args) => Debug(string.Format(message, args));

        public void Warn(string message) => Log(LogLevel.Warn, message);
        public void Warn(string message, params object[] args) => Warn(string.Format(message, args));

        public void Error(string message) => Log(LogLevel.Error, message);
        public void Error(string message, params object[] args) => Error(string.Format(message, args));
    }
}