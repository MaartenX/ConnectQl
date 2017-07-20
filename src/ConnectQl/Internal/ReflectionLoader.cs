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
        public static readonly Lazy<Func<string, UriResolveMode, Task<Stream>>> UriResolver = new Lazy<Func<string, UriResolveMode, Task<Stream>>>(TryCreateUriResolver);

        /// <summary>
        /// The plugin resolver.
        /// </summary>
        public static readonly Lazy<IPluginResolver> PluginResolver = new Lazy<IPluginResolver>(TryLoadPluginProvider);

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
        private static Func<string, UriResolveMode, Task<Stream>> TryCreateUriResolver()
        {
            Type fileType;
            Type fileModeType;

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

                var readParameters = new[]
                                         {
                                         typeof(string),
                                     };

                var writeParameters = new[]
                                          {
                                          typeof(string),
                                          fileModeType,
                                      };

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

                    return (uri, fileMode) => Task.FromResult(lambda(uri, fileMode));
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
