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

namespace ConnectQl.Intellisense.Protocol
{
    using System;
    using System.Linq;
    using System.Reflection;

    using ConnectQl.Interfaces;

    using JetBrains.Annotations;

    /// <summary>
    /// The serializable type descriptor.
    /// </summary>
    internal class SerializableTypeDescriptor : ITypeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTypeDescriptor"/> class.
        /// </summary>
        public SerializableTypeDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTypeDescriptor"/> class.
        /// </summary>
        /// <param name="argumentType">
        /// The argument type.
        /// </param>
        public SerializableTypeDescriptor([NotNull] ITypeDescriptor argumentType)
        {
            this.Interfaces = argumentType.Interfaces.Length == 0 ? null : argumentType.Interfaces.Select(i => $"{i.FullName}, {i.GetTypeInfo().Assembly.GetName().Name}").ToArray();
            this.SimplifiedType = $"{argumentType.SimplifiedType.FullName}, {argumentType.SimplifiedType.GetTypeInfo().Assembly.GetName().Name}";
        }

        /// <summary>
        /// Gets or sets the interfaces.
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// Gets or sets the simplified type.
        /// </summary>
        public string SimplifiedType { get; set; }

        /// <summary>
        /// Gets the implemented interfaces.
        /// </summary>
        [NotNull]
        Type[] ITypeDescriptor.Interfaces => (this.Interfaces?.Select(i => Type.GetType(i)).Where(i => i != null) ?? Enumerable.Empty<Type>()).ToArray();

        /// <summary>
        /// Gets the simplified type.
        /// </summary>
        [NotNull]
        Type ITypeDescriptor.SimplifiedType => Type.GetType(this.SimplifiedType) ?? typeof(object);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SerializableTypeDescriptor;

            return other != null &&
                   EnumerableComparer.Equals(this.Interfaces, other.Interfaces) &&
                   string.Equals(this.SimplifiedType, other.SimplifiedType);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EnumerableComparer.GetHashCode(this.Interfaces) * 397) ^ (this.SimplifiedType?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Converts the type descriptor to a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var typeName = ((ITypeDescriptor)this).SimplifiedType.FullName;

            switch (typeName)
            {
                case "System.Single":
                    return "float";
                case "System.Double":
                    return "double";
                case "System.String":
                    return "string";
                case "System.Object":
                    return "object";
                case "System.Boolean":
                    return "bool";
                case "System.Int16":
                    return "short";
                case "System.Int32":
                    return "int";
                case "System.Int64":
                    return "long";
                case "System.UInt16":
                    return "ushort";
                case "System.UInt32":
                    return "uint";
                case "System.UInt64":
                    return "ulong";
                case "System.DateTime":
                    return "datetime";
                case "System.TimeSpan":
                    return "timespan";
                default:
                    return typeName;
            }
        }
    }
}
