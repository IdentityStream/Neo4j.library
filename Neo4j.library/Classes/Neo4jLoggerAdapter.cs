using Microsoft.Extensions.Logging;
using System;

namespace Neo4j.library.Classes
{
    public class Neo4jLoggerAdapter : Neo4j.Driver.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public Neo4jLoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Debug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void Error(Exception cause, string message, params object[] args)
        {
            _logger.LogError(cause, message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void Warn(Exception cause, string message, params object[] args)
        {
            _logger.LogWarning(cause, message, args);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public bool IsDebugEnabled()
        {
            return _logger.IsEnabled(LogLevel.Debug);
        }

        public bool IsTraceEnabled()
        {
            return _logger.IsEnabled(LogLevel.Trace);
        }
    }
}
