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

namespace ConnectQl.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ConnectQl.Interfaces;

    /// <summary>
    /// The type descriptor.
    /// </summary>
    internal class TypeDescriptor : ITypeDescriptor
    {
        /// <summary>
        /// The assembly name of the current assembly.
        /// </summary>
        private static readonly string ThisAssemblyName = typeof(TypeDescriptor).GetTypeInfo().Assembly.FullName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptor"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public TypeDescriptor(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var interfaces = new List<TypeInfo>(type.GetTypeInfo().ImplementedInterfaces.Select(t => t.GetTypeInfo()))
                                 {
                                     typeInfo,
                                 }.Where(i => i.IsInterface);

            this.Interfaces = interfaces.Where(ti => ti.Assembly.FullName == TypeDescriptor.ThisAssemblyName).Select(t => t.AsType()).ToArray();
            this.SimplifiedType = type;
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        public Type[] Interfaces { get; }

        /// <summary>
        /// Gets the simplified type.
        /// </summary>
        public Type SimplifiedType { get; }
    }
}