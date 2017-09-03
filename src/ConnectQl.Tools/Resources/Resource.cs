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

namespace ConnectQl.Tools.Resources
{
    using System;
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Holds the resources.
    /// </summary>
    public static class Resource
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly Lazy<ResourceManager> ResourceManager = new Lazy<ResourceManager>(() => new ResourceManager("VSPackage", typeof(Resource).Assembly));

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public static CultureInfo Culture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>The resource string.</returns>
        public static string Get(string resource)
        {
            return ResourceManager.Value.GetString(resource, Culture);
        }
    }
}
