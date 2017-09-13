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

namespace ConnectQl.Internal.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The dictionary extensions.
    /// </summary>
    internal static class BinaryTypeConverter
    {
        /// <summary>
        /// Maps the operands of a binary expression to a result type.
        /// </summary>
        private static readonly Dictionary<Tuple<Type, Type>, Type> Mappings = new Dictionary<Tuple<Type, Type>, Type>()
            .Add<sbyte, byte, short>().Add<short, byte, short>().Add<ushort, byte, ushort>().Add<char, byte, char>().Add<int, byte, int>()
            .Add<uint, byte, uint>().Add<float, byte, float>().Add<long, byte, long>().Add<ulong, byte, ulong>().Add<double, byte, double>()
            .Add<decimal, byte, decimal>().Add<object, byte, object>().Add<string, byte, string>().Add<short, sbyte, short>().Add<ushort, sbyte, int>()
            .Add<char, sbyte, char>().Add<int, sbyte, int>().Add<uint, sbyte, long>().Add<float, sbyte, float>().Add<long, sbyte, long>()
            .Add<ulong, sbyte, ulong>().Add<double, sbyte, double>().Add<decimal, sbyte, decimal>().Add<object, sbyte, object>().Add<string, sbyte, string>()
            .Add<ushort, short, int>().Add<char, short, char>().Add<int, short, int>().Add<uint, short, long>().Add<float, short, float>()
            .Add<long, short, long>().Add<ulong, short, ulong>().Add<double, short, double>().Add<decimal, short, decimal>().Add<object, short, object>()
            .Add<string, short, string>().Add<char, ushort, char>().Add<int, ushort, int>().Add<uint, ushort, uint>().Add<float, ushort, float>()
            .Add<long, ushort, long>().Add<ulong, ushort, ulong>().Add<double, ushort, double>().Add<decimal, ushort, decimal>().Add<object, ushort, object>()
            .Add<string, ushort, string>().Add<int, char, int>().Add<uint, char, uint>().Add<float, char, float>().Add<long, char, long>()
            .Add<ulong, char, ulong>().Add<double, char, double>().Add<decimal, char, decimal>().Add<object, char, object>().Add<string, char, string>()
            .Add<uint, int, long>().Add<float, int, float>().Add<long, int, long>().Add<ulong, int, ulong>().Add<double, int, double>()
            .Add<decimal, int, decimal>().Add<object, int, object>().Add<string, int, string>().Add<float, uint, float>().Add<long, uint, long>()
            .Add<ulong, uint, ulong>().Add<double, uint, double>().Add<decimal, uint, decimal>().Add<object, uint, object>().Add<string, uint, string>()
            .Add<long, float, float>().Add<ulong, float, float>().Add<double, float, double>().Add<decimal, float, decimal>().Add<object, float, object>()
            .Add<string, float, string>().Add<ulong, long, long>().Add<double, long, double>().Add<decimal, long, decimal>().Add<object, long, object>()
            .Add<string, long, string>().Add<double, ulong, double>().Add<decimal, ulong, decimal>().Add<object, ulong, object>().Add<string, ulong, string>()
            .Add<decimal, double, decimal>().Add<object, decimal, object>().Add<string, decimal, string>().Add<string, object, string>();

        /// <summary>
        /// Gets the result type for the two operand types.
        /// </summary>
        /// <param name="first">
        /// The first operand.
        /// </param>
        /// <param name="second">
        /// The second operand.
        /// </param>
        /// <returns>
        /// The result type or <c>null</c> if no type conversion was possible.
        /// </returns>
        public static Type GetResultType(Type first, Type second)
        {
            return first == second ? first : BinaryTypeConverter.Mappings.TryGetValue(Tuple.Create(first, second), out Type result) ? result : null;
        }

        /// <summary>
        /// Adds a mapping to the dictionary. It adds a mapping from {<typeparamref name="TOperand1"/>,
        ///     <typeparamref name="TOperand2"/>} to <typeparamref name="TResult"/> and also a
        ///     mapping from {<typeparamref name="TOperand2"/>, <typeparamref name="TOperand1"/>} to
        ///     <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <typeparam name="TOperand1">
        /// The type of the first operand.
        /// </typeparam>
        /// <typeparam name="TOperand2">
        /// The type of the second operand.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The result type.
        /// </typeparam>
        /// <returns>
        /// The <paramref name="dictionary"/>.
        /// </returns>
        private static Dictionary<Tuple<Type, Type>, Type> Add<TOperand1, TOperand2, TResult>(this Dictionary<Tuple<Type, Type>, Type> dictionary)
        {
            dictionary.Add(Tuple.Create(typeof(TOperand1), typeof(TOperand2)), typeof(TResult));
            dictionary.Add(Tuple.Create(typeof(TOperand2), typeof(TOperand1)), typeof(TResult));

            return dictionary;
        }
    }
}