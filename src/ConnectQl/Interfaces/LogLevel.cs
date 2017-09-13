namespace ConnectQl.Interfaces
{
    public enum LogLevel
    {
        /// <summary>
        /// Logs a message using debug-level.
        /// </summary>
        Debug,

        /// <summary>
        /// Logs a message using verbose-level.
        /// </summary>
        Verbose,

        /// <summary>
        /// Logs a message using information-level.
        /// </summary>
        Information,

        /// <summary>
        /// Logs a message using warning-level.
        /// </summary>
        Warning,

        /// <summary>
        /// Logs a message using error-level.
        /// </summary>
        Error
    }
}