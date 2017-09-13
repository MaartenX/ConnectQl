namespace ConnectQl.Logger.Console
{
    using System;
    using ConnectQl.Interfaces;

    /// <summary>
    /// Logs to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Writes a message to the logger.
        /// </summary>
        /// <param name="logLevel">The log level to write.</param>
        /// <param name="exception">Optinonal. The <see cref="System.Exception"/> if there was one, <c>null</c> otherwise.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments.</param>
        public void Write(LogLevel logLevel, Exception exception, string format = "", params object[] args)
        {
            var color = ConsoleColor.Gray;

            switch (logLevel)
            {
                case LogLevel.Debug:
                    color = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Verbose:
                    color = ConsoleColor.Gray;
                    break;
                case LogLevel.Warning:
                    color = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;
            }

            var previous = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;

                if (format == null && exception != null)
                {
                    format = exception.Message;
                    args = new object[0];
                }

                Console.WriteLine(format, args);
            }
            finally
            {
                Console.ForegroundColor = previous;
            }
        }
    }
}