namespace ConnectQl.Interfaces
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Extensions for the loggeri nterface.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Writes a debug-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Debug, exception, format, args);
        }

        /// <summary>
        /// Writes a debug-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        [Conditional("DEBUG")]
        public static void Debug(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Debug, null, format, args);
        }

        /// <summary>
        /// Writes a verbose-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Verbose(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Verbose, exception, format, args);
        }

        /// <summary>
        /// Writes a verbose-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Verbose(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Verbose, null, format, args);
        }

        /// <summary>
        /// Writes a information-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Information(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Information, exception, format, args);
        }

        /// <summary>
        /// Writes a information-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Information(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Information, null, format, args);
        }

        /// <summary>
        /// Writes a warning-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Warning(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Warning, exception, format, args);
        }

        /// <summary>
        /// Writes a warning-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Warning(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Warning, null, format, args);
        }

        /// <summary>
        /// Writes a error-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to write.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Error(this ILogger logger, Exception exception, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Error, exception, format, args);
        }

        /// <summary>
        /// Writes a error-level message to the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The message to write.</param>
        /// <param name="args">The message arguments.</param>
        public static void Error(this ILogger logger, string format = "", params object[] args)
        {
            logger.Write(LogLevel.Error, null, format, args);
        }
    }
}