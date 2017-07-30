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

namespace ConnectQl.Utilities
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Marks a property as a field for a connection string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited =true)]
    public class ConnectionStringPartAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringPartAttribute"/> class.
        /// Marks the property as a part of the connection string.
        /// </summary>
        public ConnectionStringPartAttribute()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringPartAttribute"/> class.
        /// Marks the property as a part of the connection string.
        /// </summary>
        /// <param name="alternateNames">The alternate names of the part.</param>
        public ConnectionStringPartAttribute(params string[] alternateNames)
            : this(false, alternateNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringPartAttribute"/> class.
        /// Marks the property as a part of the connection string.
        /// </summary>
        /// <param name="ignorePropertyName">if set to <c>true</c> ignores the name of the property in the connection string.</param>
        /// <param name="alternateNames">The alternate names of the part.</param>
        public ConnectionStringPartAttribute(bool ignorePropertyName, params string[] alternateNames)
        {
            this.IgnorePropertyName = ignorePropertyName;
            this.Names = alternateNames;
        }

        /// <summary>
        /// Gets the names of the property in the connection string.
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// Gets a value indicating whether to ignore the name of the property in the connection string and use only the alternate names.
        /// </summary>
        public bool IgnorePropertyName { get; }

        /// <summary>
        /// Gets the names that are valid in the connection string for the specified property.
        /// </summary>
        /// <param name="propertyInfo">The property to get the names for.</param>
        /// <returns>An array of valid names for the property.</returns>
        internal string[] GetNames(PropertyInfo propertyInfo)
        {
            var result = this.Names.DefaultIfEmpty(propertyInfo.Name).ToList();

            if (!this.IgnorePropertyName)
            {
                result.Add(propertyInfo.Name);
            }

            return result.Distinct().ToArray();
        }
    }
}
