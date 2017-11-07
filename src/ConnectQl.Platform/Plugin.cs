// MIT License
//
// Copyright (c) 2017 Maarten van Sambeek.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace ConnectQl.Platform
{
#if NET45
    using System.Configuration;
#endif

    using ConnectQl.FileFormats;
    using ConnectQl.Interfaces;
    using ConnectQl.Platform.FileFormats;

    using JetBrains.Annotations;

    /// <summary>
    /// The platform plugin.
    /// </summary>
    [PublicAPI]
    public class Plugin : IConnectQlPlugin
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => "Platform";

        /// <summary>
        /// Registers the plugin on the context.
        /// </summary>
        /// <param name="context">
        /// The context to register the plugin on.
        /// </param>
        public void RegisterPlugin(IPluginContext context)
        {
            context.Functions
                .AddWithoutSideEffects("connectionstring", (string name) => ConnectionString(name))
                .SetDescription("Gets a connection string from the application config file.", "The name of the connection string.")
                .AddWithoutSideEffects("connectionstring", (string name, string configFile) => ConnectionString(name, configFile))
                .SetDescription("Gets a connection string from a config file.", "The name of the connection string.", "The location of the config file.")
                .AddWithoutSideEffects("appsetting", (string name) => AppSetting(name))
                .SetDescription("Gets an application setting from the application config file.", "The name of the application setting.")
                .AddWithoutSideEffects("separator", (string separator) => separator)
                .SetDescription("Sets the separator for the CSV file format.", "The separator to use (default: ',').");

            context.FileFormats
                .Add(new CsvFileFormat())
                .Add(new JsonFileFormat());
        }

        /// <summary>
        /// Gets a connection string from the configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of the connection string.
        /// </param>
        /// <returns>
        /// The connection string or <c>null</c> if it was not found.
        /// </returns>
        private static string ConnectionString(string name)
        {
#if NETSTANDARD1_5
            return string.Empty;
#elif NET45
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
#endif
        }

        /// <summary>
        /// Gets a connection string from the specified configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of the connection string.
        /// </param>
        /// <param name="configFile">
        /// The location of the config file.
        /// </param>
        /// <returns>
        /// The connection string or <c>null</c> if it was not found.
        /// </returns>
        private static string ConnectionString(string name, string configFile)
        {
#if NETSTANDARD1_5
            return string.Empty;
#elif NET45
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFile
                },
                ConfigurationUserLevel.None);

            return config.ConnectionStrings.ConnectionStrings[name]?.ConnectionString;
#endif
        }

        /// <summary>
        /// Gets a application setting from the configuration file.
        /// </summary>
        /// <param name="name">
        /// The name of the connection string.
        /// </param>
        /// <returns>
        /// The connection string or <c>null</c> if it was not found.
        /// </returns>
        private static string AppSetting(string name)
        {
#if NETSTANDARD1_5
            return string.Empty;
#elif NET45
            return ConfigurationManager.AppSettings[name];
#endif
        }
    }
}