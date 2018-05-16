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

namespace ConnectQl.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extensions for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the base type if the type implements it.
        /// </summary>
        /// <param name="type">
        /// The type to check.
        /// </param>
        /// <param name="baseType">
        /// The base type to check for, can be a generic type definition.
        /// </param>
        /// <returns>
        /// The type.
        /// </returns>
        public static Type GetBaseType(this Type type, Type baseType)
        {
            if (baseType.GetTypeInfo().IsInterface)
            {
                for (; type != typeof(object); type = type.GetTypeInfo().BaseType)
                {
                    var iface = type.GetTypeInfo().GetInterfaces().FirstOrDefault(i => i == baseType || i.IsConstructedGenericType && i.GetGenericTypeDefinition() == baseType);

                    if (iface != null)
                    {
                        return iface;
                    }
                }
            }
            else
            {
                for (; type != typeof(object); type = type.GetTypeInfo().BaseType)
                {
                    if (type == baseType || type.IsConstructedGenericType && type.GetGenericTypeDefinition() == baseType)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
