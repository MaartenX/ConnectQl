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

namespace ConnectQl.Internal
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using ConnectQl.Interfaces;

    /// <summary>
    /// The reflection loader.
    /// </summary>
    internal static class ReflectionLoader
    {
        /// <summary>
        /// The URI resolver.
        /// </summary>
        public static readonly Lazy<IUriResolver> UriResolver = new Lazy<IUriResolver>(ReflectionLoader.TryCreateUriResolver);

        /// <summary>
        /// The plugin resolver.
        /// </summary>
        public static readonly Lazy<IPluginResolver> PluginResolver = new Lazy<IPluginResolver>(ReflectionLoader.TryLoadPluginProvider);

        /// <summary>
        ///     Tries to load the plugin resolver using reflection by checking if the ConnectQl.Platform assembly is loaded.
        /// </summary>
        /// <returns>
        ///     The <see cref="IPluginResolver" />.
        /// </returns>
        private static IPluginResolver TryLoadPluginProvider()
        {
            try
            {
                var currentName = typeof(ConnectQlContext).GetTypeInfo().Assembly.GetName();

                var name = new AssemblyName("ConnectQl.Platform");

                name.SetPublicKey(currentName.GetPublicKey());

                var providerType = Assembly.Load(name).DefinedTypes.FirstOrDefault(type => type.ImplementedInterfaces.Any(i => i.FullName == typeof(IPluginResolver).FullName));

                if (providerType != null)
                {
                    return Activator.CreateInstance(providerType.AsType()) as IPluginResolver;
                }
            }
            catch
            {
                //// Ignore.
            }

            return null;
        }

        /// <summary>
        ///     Tries to create an uri resolver. We do this by cheating; NetStandard1.0 and the PCL do not support
        ///     file system operations but we can always try and load them via reflection (since the calling assembly is probably running on a platform that supports files).
        ///     When this fails no Uri resolver is created, but at least we tried...
        /// </summary>
        /// <returns>
        /// The UriResolveMode
        /// </returns>
        private static IUriResolver TryCreateUriResolver()
        {
            Type fileType, fileModeType;

            try
            {
#if NETSTANDARD1_0
                var fs = Assembly.Load(new AssemblyName("System.IO.FileSystem"));
                var fsp = Assembly.Load(new AssemblyName("System.IO.FileSystem.Primitives"));

                fileType = fs?.GetType("System.IO.File");
                fileModeType = fsp?.GetType("System.IO.FileMode");
#else
                fileType = Type.GetType("System.IO.File");
                fileModeType = Type.GetType("System.IO.FileMode");
#endif

                if (fileType == null || fileModeType == null)
                {
                    return null;
                }

                var uriParameter = Expression.Parameter(typeof(string), "uri");
                var fileModeParameter = Expression.Parameter(typeof(UriResolveMode), "fileMode");

                var fullPathParameters = new[]
                {
                    typeof(string)
                };

                var readParameters = new[]
                                         {
                                         typeof(string),
                                     };

                var writeParameters = new[]
                                          {
                                          typeof(string),
                                          fileModeType,
                                      };

                var getFullPathMethod = typeof(Path).GetRuntimeMethod("GetFullPath", fullPathParameters);
                var openReadMethod = fileType.GetRuntimeMethod("OpenRead", readParameters);
                var openMethod = fileType.GetRuntimeMethod("Open", writeParameters);
                var createField = fileModeType.GetRuntimeField("Create")?.GetValue(null);

                if (openReadMethod != null && openMethod != null && createField != null)
                {
                    var lambda = Expression.Lambda<Func<string, UriResolveMode, Stream>>(
                            Expression.Condition(
                                Expression.Equal(fileModeParameter, Expression.Constant(UriResolveMode.Read)),
                                Expression.Call(null, openReadMethod, uriParameter),
                                Expression.Call(null, openMethod, uriParameter, Expression.Constant(createField))),
                            uriParameter,
                            fileModeParameter)
                        .Compile();

                    var getFullPath = Expression.Lambda<Func<string, string>>(Expression.Call(getFullPathMethod, uriParameter), uriParameter).Compile();

                    return new UriResolverImplementation(getFullPath, (uri, fileMode) => Task.FromResult(lambda(uri, fileMode)));
                }
            }
            catch
            {
            }

            return null;
        }

        private class UriResolverImplementation : IUriResolver
        {
            /// <summary>A lambda that returns the full path for a file.</summary>
            private readonly Func<string, string> getFullPath;

            /// <summary>A lambda that resolves the uri to a stream.</summary>
            private readonly Func<string, UriResolveMode, Task<Stream>> resolveToStream;

            /// <summary>
            /// Initializes a new instance of the <see cref="UriResolverImplementation"/> class.
            /// </summary>
            /// <param name="getFullPath">A lambda that returns the full path for a file.</param>
            /// <param name="resolveToStream">A lambda that resolves the uri to a stream.</param>
            public UriResolverImplementation(Func<string, string> getFullPath, Func<string, UriResolveMode, Task<Stream>> resolveToStream)
            {
                this.getFullPath = getFullPath;
                this.resolveToStream = resolveToStream;
            }

            /// <summary>
            /// Gets the full path of the uri.
            /// </summary>
            /// <param name="uri">The uri to get the full path for.</param>
            /// <returns>
            /// The full path of the uri.
            /// </returns>
            public string GetFullPath(string uri) => this.getFullPath(uri);

            /// <summary>
            /// Resolves an uri to a stream.
            /// </summary>
            /// <param name="uri">The uri to resolve.</param>
            /// <param name="mode">The mode to use when resolving the uri.</param>
            /// <returns>A Task returning a stream.</returns>
            public Task<Stream> ResolveToStream(string uri, UriResolveMode mode) => this.resolveToStream(uri, mode);
        }
    }
}
