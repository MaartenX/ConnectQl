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
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Base class for connection strings.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the inheriting connection string.
    /// </typeparam>
    public abstract class ConnectionStringBase<T>
        where T : new()
    {
        /// <summary>
        /// Converts connection string names to properties.
        /// </summary>
        private static readonly Dictionary<string, PropertyInfo> NameToProperty;

        /// <summary>
        /// Converts properties to names.
        /// </summary>
        private static readonly Dictionary<PropertyInfo, string> PropertyToName;

        /// <summary>
        /// Stores the exception when initialization fails.
        /// </summary>
        private static readonly Exception Exception;

        /// <summary>
        /// Initializes static members of the <see cref="ConnectionStringBase{T}"/> class.
        /// </summary>
        static ConnectionStringBase()
        {
            try
            {
                var propertyAttributes =
                    typeof(T)
                        .GetRuntimeProperties()
                        .Where(prop => prop.CanRead && prop.CanWrite && prop.GetMethod.IsPublic && prop.SetMethod.IsPublic)
                        .Select(prop => new { Property = prop, Names = prop.GetCustomAttribute<ConnectionStringPartAttribute>()?.GetNames(prop) })
                        .Where(pa => pa.Names != null)
                        .ToList();

                NameToProperty = propertyAttributes.SelectMany(pa => pa.Names.Select(name => new { Name = name, pa.Property })).ToDictionary(np => np.Name, np => np.Property, StringComparer.OrdinalIgnoreCase);
                PropertyToName = propertyAttributes.ToDictionary(pa => pa.Property, pa => pa.Names.First());
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }

        /// <summary>
        /// Parses the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to parse.</param>
        /// <returns>
        /// The parsed connection string.
        /// </returns>
        /// <exception cref="ParseException">Thrown when parsing fails.</exception>
        public static T Parse(string connectionString)
        {
            if (Exception != null)
            {
                throw new ParseException(Exception);
            }

            try
            {
                var values = new DbConnectionStringBuilder() { ConnectionString = connectionString }.Cast<KeyValuePair<string, object>>();
                var result = new T();

                foreach (var keyValue in values)
                {
                    if (NameToProperty.TryGetValue(keyValue.Key, out var propertyInfo))
                    {
                        var value = ConvertTo(keyValue.Value, propertyInfo.PropertyType);

                        if (value != null)
                        {
                            propertyInfo.SetValue(result, value);
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new ParseException(e);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var values = PropertyToName
                .Select(pn =>
                    {
                        var value = pn.Key.GetValue(this);

                        return new KeyValuePair<string, object>(pn.Value, value == Default(pn.Key.PropertyType) ? null : value);
                    })
                .Where(kv => kv.Value != null);

            var builder = new DbConnectionStringBuilder();

            foreach (var keyValue in values)
            {
                builder.Add(keyValue.Key, keyValue.Value);
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// Gets the default value for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The default value.
        /// </returns>
        private static object Default(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Converts the value to the specified type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>The converted value, or <c>null</c> if conversion isn't possible.</returns>
        private static object ConvertTo(object value, Type type)
        {
            if (type == typeof(Uri))
            {
                return new Uri((string)ConvertTo(value, typeof(string)));
            }

            if (type == typeof(Guid))
            {
                return Guid.TryParse((string)ConvertTo(value, typeof(string)), out var result) ? (object)result : null;
            }

            try
            {
                return Convert.ChangeType(value, type);
            }
            catch
            {
                return null;
            }
        }
    }
}
