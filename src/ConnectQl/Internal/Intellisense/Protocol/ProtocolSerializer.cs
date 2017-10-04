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

namespace ConnectQl.Internal.Intellisense.Protocol
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using ConnectQl.Internal.Extensions;

    using JetBrains.Annotations;

    /// <summary>
    /// The protocol serializer.
    /// </summary>
    internal static class ProtocolSerializer
    {
        /// <summary>
        /// The <see cref="Stream.Write(byte[], int, int)"/>method.
        /// </summary>
        private static readonly MethodInfo StreamWriteMethod = ((MethodCallExpression)((Expression<Action<Stream>>)(s => s.Write(new byte[0], 0, 0))).Body).Method;

        /// <summary>
        /// The <see cref="Stream.Read(byte[], int, int)"/>method.
        /// </summary>
        private static readonly MethodInfo StreamReadMethod = ((MethodCallExpression)((Expression<Action<Stream>>)(s => s.Read(new byte[0], 0, 0))).Body).Method;

        /// <summary>
        /// The <see cref="Stream.WriteByte(byte)"/>method.
        /// </summary>
        private static readonly MethodInfo StreamWriteByteMethod = ((MethodCallExpression)((Expression<Action<Stream>>)(s => s.WriteByte(0))).Body).Method;

        /// <summary>
        /// The <see cref="Stream.ReadByte"/> method.
        /// </summary>
        private static readonly MethodInfo StreamReadByteMethod = ((MethodCallExpression)((Expression<Action<Stream>>)(s => s.ReadByte())).Body).Method;

        /// <summary>
        /// The <see cref="Encoding.GetString(byte[], int, int)"/> method.
        /// </summary>
        private static readonly MethodInfo EncodingGetStringMethod = ((MethodCallExpression)((Expression<Action<Encoding>>)(s => s.GetString(new byte[0], 0, 0))).Body).Method;

        /// <summary>
        /// The <see cref="Encoding.GetBytes(string)"/> method.
        /// </summary>
        private static readonly MethodInfo EncodingGetBytesMethod = ((MethodCallExpression)((Expression<Action<Encoding>>)(s => s.GetBytes(string.Empty))).Body).Method;

        /// <summary>
        /// Deserializes the specified serialized value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value to deserialize.
        /// </typeparam>
        /// <param name="serialized">The serialized value.</param>
        /// <returns>
        /// The value.
        /// </returns>
        /// <exception cref="ProtocolSerializerException">
        /// Thrown when initialization fails.
        /// </exception>
        public static T Deserialize<T>(byte[] serialized)
        {
            using (var ms = new MemoryStream(serialized))
            {
                if (ProtocolSerializerImplementation<T>.InitializeError != null)
                {
                    throw new ProtocolSerializerException(ProtocolSerializerImplementation<T>.InitializeError);
                }

                return ProtocolSerializerImplementation<T>.Read(ms);
            }
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The value serialized in a byte arry.</returns>
        /// <exception cref="ProtocolSerializerException">
        /// Thrown when initialization fails.
        /// </exception>
        public static byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                if (ProtocolSerializerImplementation<T>.InitializeError != null)
                {
                    throw new ProtocolSerializerException(ProtocolSerializerImplementation<T>.InitializeError);
                }

                ProtocolSerializerImplementation<T>.Write(ms, value);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Creates an expression that reads the specified number of bytes from the stream.
        /// </summary>
        /// <param name="stream">
        /// The lambda parameter that holds the stream.
        /// </param>
        /// <param name="length">
        /// The number of bytes to read.
        /// </param>
        /// <returns>
        /// An expression returning a byte array.
        /// </returns>
        private static Expression ReadBytes(ParameterExpression stream, Expression length)
        {
            var bytes = Expression.Parameter(typeof(byte[]));

            return Expression.Block(
                new[] { bytes },
                Expression.Assign(bytes, Expression.NewArrayBounds(typeof(byte), length)),
                Expression.Call(stream, ProtocolSerializer.StreamReadMethod, bytes, Expression.Constant(0), Expression.Property(bytes, nameof(Array.Length))),
                bytes);
        }

        /// <summary>
        /// Creates an expression that reads a string from a stream.
        /// </summary>
        /// <param name="stream">
        /// The lambda parameter that holds the stream.
        /// </param>
        /// <returns>
        /// An expression that returns the string.
        /// </returns>
        private static Expression ReadString(ParameterExpression stream)
        {
            var bytes = Expression.Parameter(typeof(byte[]));

            var r = Expression.Condition(
                ProtocolSerializer.ReadValue(stream, typeof(bool)),
                Expression.Block(
                    new[] { bytes },
                    Expression.Assign(bytes, ProtocolSerializer.ReadBytes(stream, ProtocolSerializer.ReadValue(stream, typeof(int)))),
                    Expression.Call(
                        Expression.Constant(Encoding.UTF8),
                        ProtocolSerializer.EncodingGetStringMethod,
                        bytes,
                        Expression.Constant(0),
                        Expression.Property(bytes, nameof(Array.Length)))),
                Expression.Constant(null, typeof(string)));

            return r;
        }

        /// <summary>
        /// Creates an expression that writes a string to a stream.
        /// </summary>
        /// <param name="stream">
        /// The lambda parameter that holds the stream.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        /// <returns>
        /// An expression that writes the string.
        /// </returns>
        private static Expression WriteString(ParameterExpression stream, Expression value)
        {
            var valueIsNotNull = Expression.Variable(typeof(bool));
            var bytes = Expression.Variable(typeof(byte[]));

            return Expression.Block(
                Expression.IfThenElse(
                    Expression.Equal(value, Expression.Constant(null, typeof(string))),
                    ProtocolSerializer.WriteValue(stream, Expression.Constant(false)),
                    Expression.Block(
                        new[] { bytes },
                        ProtocolSerializer.WriteValue(stream, Expression.Constant(true)),
                        ProtocolSerializer.WriteValue(stream, Expression.Property(value, nameof(string.Length))),
                        Expression.Assign(bytes, Expression.Call(Expression.Constant(Encoding.UTF8), ProtocolSerializer.EncodingGetBytesMethod, value)),
                        Expression.Call(stream, ProtocolSerializer.StreamWriteMethod, bytes, Expression.Constant(0), Expression.Property(bytes, nameof(Array.Length))))));
        }

        /// <summary>
        /// Creates an expression that reads and calls the bit converter.
        /// </summary>
        /// <typeparam name="T">
        /// The type to convert to.
        /// </typeparam>
        /// <param name="bitConverter">The bit converter.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="sizeInBytes">The size in bytes.</param>
        /// <returns>
        /// The expression.
        /// </returns>
        private static Expression ReadAndCallBitConverter<T>(Func<byte[], int, T> bitConverter, ParameterExpression stream, int sizeInBytes)
        {
            return Expression.Call(bitConverter.GetMethodInfo(), ProtocolSerializer.ReadBytes(stream, Expression.Constant(sizeInBytes)), Expression.Constant(0));
        }

        /// <summary>
        /// Creates an expression that writes the bytes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value to write.
        /// </typeparam>
        /// <param name="bitConverter">The bit converter.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The expression.
        /// </returns>
        private static Expression WriteBytes<T>(Func<T, byte[]> bitConverter, ParameterExpression stream, Expression value)
        {
            var bytes = Expression.Parameter(typeof(byte[]));

            return Expression.Block(
                new[] { bytes },
                Expression.Assign(bytes, Expression.Call(bitConverter.GetMethodInfo(), value)),
                Expression.Call(stream, ProtocolSerializer.StreamWriteMethod, bytes, Expression.Constant(0), Expression.Property(bytes, nameof(Array.Length))));
        }

        /// <summary>
        /// Writes an expression that reads the object.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The expression.
        /// </returns>
        private static Expression ReadObject(ParameterExpression stream, Type type)
        {
            var obj = Expression.Parameter(type);
            var properties = type
                .GetRuntimeProperties()
                .Where(prop => prop.GetMethod?.IsStatic == false && prop.GetMethod?.IsPublic == true && prop.SetMethod?.IsPublic == true && prop.GetCustomAttribute<NotInProtocolAttribute>() == null)
                .OrderBy(p => p.Name);

            var reader = new List<Expression>()
            {
                Expression.Assign(obj, Expression.New(type))
            };

            reader.AddRange(properties.Select(p => Expression.Assign(Expression.Property(obj, p), ProtocolSerializer.ReadValue(stream, p.PropertyType))));
            reader.Add(obj);

            return Expression.Condition(
                    ProtocolSerializer.ReadValue(stream, typeof(bool)),
                    Expression.Block(new[] { obj }, reader),
                    Expression.Constant(null, type));
        }

        /// <summary>
        /// Creates an expression that writes the object.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The expression.
        /// </returns>
        private static Expression WriteObject(ParameterExpression stream, [NotNull] Expression value)
        {
            var obj = Expression.Parameter(value.Type);
            var valueIsNotNull = Expression.Parameter(typeof(bool));
            var properties = value.Type
                .GetRuntimeProperties()
                .Where(prop => prop.GetMethod?.IsStatic == false && prop.GetMethod?.IsPublic == true && prop.SetMethod?.IsPublic == true && prop.GetCustomAttribute<NotInProtocolAttribute>() == null)
                .OrderBy(p => p.Name);

            var writes = properties.Select(p => ProtocolSerializer.WriteValue(stream, Expression.Property(obj, p))).ToList();

            writes.Insert(0, ProtocolSerializer.WriteValue(stream, Expression.Constant(true)));

            return Expression.Block(
                new[] { obj },
                Expression.Assign(obj, value),
                Expression.IfThenElse(
                    Expression.Equal(obj, Expression.Constant(null, value.Type)),
                    ProtocolSerializer.WriteValue(stream, Expression.Constant(false)),
                    Expression.Block(writes)));
        }

        /// <summary>
        /// Creates an expression that writes the value.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The expression.
        /// </returns>
        private static Expression WriteValue(ParameterExpression stream, [NotNull] Expression value)
        {
            if (value.Type == typeof(string))
            {
                return ProtocolSerializer.WriteString(stream, value);
            }

            var enumerable = value.Type.GetInterface(typeof(IEnumerable<>));

            if (enumerable != null)
            {
                return ProtocolSerializer.WriteEnumerable(stream, value);
            }

            if (value.Type.GetTypeInfo().IsEnum)
            {
                return ProtocolSerializer.WriteValue(stream, Expression.Convert(value, Enum.GetUnderlyingType(value.Type)));
            }

            if (value.Type == typeof(ushort))
            {
                return ProtocolSerializer.WriteBytes((Func<ushort, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(uint))
            {
                return ProtocolSerializer.WriteBytes((Func<uint, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(ulong))
            {
                return ProtocolSerializer.WriteBytes((Func<ulong, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(char))
            {
                return ProtocolSerializer.WriteBytes((Func<char, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(long))
            {
                return ProtocolSerializer.WriteBytes((Func<long, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(int))
            {
                return ProtocolSerializer.WriteBytes((Func<int, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(float))
            {
                return ProtocolSerializer.WriteBytes((Func<float, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(double))
            {
                return ProtocolSerializer.WriteBytes((Func<double, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(short))
            {
                return ProtocolSerializer.WriteBytes((Func<short, byte[]>)BitConverter.GetBytes, stream, value);
            }

            if (value.Type == typeof(byte))
            {
                return Expression.Call(stream, ProtocolSerializer.StreamWriteByteMethod, value);
            }

            if (value.Type == typeof(bool))
            {
                return value is ConstantExpression constant
                    ? Expression.Call(stream, ProtocolSerializer.StreamWriteByteMethod, (bool)constant.Value == true ? Expression.Constant((byte)1) : Expression.Constant((byte)0))
                    : Expression.Call(stream, ProtocolSerializer.StreamWriteByteMethod, Expression.Condition(value, Expression.Constant((byte)1), Expression.Constant((byte)0)));
            }

            return ProtocolSerializer.WriteObject(stream, value);
        }

        /// <summary>
        /// Creates an expression that writes the enumerable.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <returns>The expression.</returns>
        private static Expression WriteEnumerable(ParameterExpression stream, [NotNull] Expression value)
        {
            var collection = value.Type.GetInterface(typeof(ICollection<>));

            if (collection != null)
            {
                var elementType = collection.GetTypeInfo().GenericTypeArguments[0];
                var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
                var enumerator = Expression.Parameter(enumeratorType);
                var breakTarget = Expression.Label();

                var collectionGetEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(elementType).GetMethod(nameof(IEnumerable<object>.GetEnumerator));
                var enumeratorMoveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));

                return Expression.IfThenElse(
                    Expression.Equal(value, Expression.Constant(null, value.Type)),
                    ProtocolSerializer.WriteValue(stream, Expression.Constant(false)),
                    Expression.Block(
                        new[] { enumerator },
                        ProtocolSerializer.WriteValue(stream, Expression.Constant(true)),
                        ProtocolSerializer.WriteValue(stream, Expression.Property(Expression.Convert(value, collection), nameof(ICollection<int>.Count))),
                        Expression.Assign(enumerator, Expression.Call(Expression.Convert(value, collection), collectionGetEnumeratorMethod)),
                        Expression.Loop(
                                Expression.IfThenElse(
                                    Expression.Call(enumerator, enumeratorMoveNextMethod),
                                    ProtocolSerializer.WriteValue(stream, Expression.Property(enumerator, nameof(IEnumerator<object>.Current))),
                                    Expression.Break(breakTarget)),
                            breakTarget)));
            }

            return Expression.Constant(null);
        }

        /// <summary>
        /// Generates an expression that reads a item from the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to read from.
        /// </param>
        /// <param name="type">
        /// The type of object to read.
        /// </param>
        /// <returns>
        /// An expression that returns the value from a stream.
        /// </returns>
        private static Expression ReadValue(ParameterExpression stream, Type type)
        {
            if (type == typeof(string))
            {
                return ProtocolSerializer.ReadString(stream);
            }

            var enumerable = type.GetInterface(typeof(IEnumerable<>));

            if (enumerable != null)
            {
                return ProtocolSerializer.ReadEnumerable(stream, type, enumerable);
            }

            if (type.GetTypeInfo().IsEnum)
            {
                return Expression.Convert(ProtocolSerializer.ReadValue(stream, Enum.GetUnderlyingType(type)), type);
            }

            if (type == typeof(char))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToChar, stream, 2);
            }

            if (type == typeof(ulong))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToUInt64, stream, 8);
            }

            if (type == typeof(uint))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToUInt32, stream, 8);
            }

            if (type == typeof(ushort))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToUInt16, stream, 8);
            }

            if (type == typeof(long))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToInt64, stream, 8);
            }

            if (type == typeof(int))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToInt32, stream, 4);
            }

            if (type == typeof(float))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToSingle, stream, 4);
            }

            if (type == typeof(double))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToDouble, stream, 8);
            }

            if (type == typeof(short))
            {
                return ProtocolSerializer.ReadAndCallBitConverter(BitConverter.ToInt16, stream, 2);
            }

            if (type == typeof(byte))
            {
                return Expression.Call(stream, ProtocolSerializer.StreamReadByteMethod);
            }

            if (type == typeof(bool))
            {
                return Expression.NotEqual(Expression.Call(stream, ProtocolSerializer.StreamReadByteMethod), Expression.Constant(0));
            }

            return ProtocolSerializer.ReadObject(stream, type);
        }

        /// <summary>
        /// Reads the enumerable.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <param name="enumerableType">Type of the enumerable.</param>
        /// <returns>An expression that reads an enumerable from a stream.</returns>
        private static Expression ReadEnumerable(ParameterExpression stream, Type type, Type enumerableType)
        {
            var typeInfo = type.GetTypeInfo();
            var elementType = enumerableType.GetTypeInfo().GenericTypeArguments[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = Expression.Parameter(typeof(IList<>).MakeGenericType(elementType));
            var length = Expression.Parameter(typeof(int));
            var i = Expression.Parameter(typeof(int));
            var exitLoop = Expression.Label();
            var isArray = false;

            Expression createList;

            if (typeInfo.IsArray)
            {
                createList = Expression.NewArrayBounds(elementType, length);
                isArray = true;
            }
            else if (typeInfo.IsInterface && listType.HasInterface(type))
            {
                createList = Expression.New(listType.GetTypeInfo().DeclaredConstructors.First(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(int) })), length);
            }
            else if (listType.HasInterface(list.Type) && listType.GetTypeInfo().DeclaredConstructors.Any(c => c.GetParameters().Length == 0))
            {
                createList = Expression.New(listType);
            }
            else
            {
                throw new InvalidOperationException($"Invalid list type: {type}.");
            }

            var addMethod = typeof(ICollection<>).MakeGenericType(elementType).GetMethod("Add", elementType);

            var addToList = isArray
                          ? (Expression)Expression.Assign(Expression.MakeIndex(list, list.Type.GetRuntimeProperty("Item"), new[] { i }), ProtocolSerializer.ReadValue(stream, elementType))
                          : Expression.Call(list, addMethod, ProtocolSerializer.ReadValue(stream, elementType));

            var result = Expression.Parameter(createList.Type);
            return Expression.Condition(
                ProtocolSerializer.ReadValue(stream, typeof(bool)),
                Expression.Block(
                    new ParameterExpression[]
                    {
                                list,
                                length,
                                i,
                                result
                    },
                    Expression.Assign(i, Expression.Constant(0)),
                    Expression.Assign(length, ProtocolSerializer.ReadValue(stream, typeof(int))),
                    Expression.Assign(list, Expression.Assign(result, createList)),
                    Expression.Loop(
                        Expression.Block(
                          Expression.IfThen(Expression.Equal(i, length), Expression.Break(exitLoop)),
                          addToList,
                          Expression.PostIncrementAssign(i)),
                        exitLoop),
                    result),
                Expression.Constant(null, result.Type));
        }

        /// <summary>
        /// Implements the procol serializer for a specific type.
        /// </summary>
        /// <typeparam name="T">The type of the type.</typeparam>
        private class ProtocolSerializerImplementation<T>
        {
            /// <summary>
            /// Lambda that read an object from the stream.
            /// </summary>
            public static readonly Func<Stream, T> Read;

            /// <summary>
            /// Lambda that writes an object to the stream.
            /// </summary>
            public static readonly Action<Stream, T> Write;

            /// <summary>
            /// Contains the error when initialization fails.
            /// </summary>
            public static readonly Exception InitializeError;

            /// <summary>
            /// Initializes static members of the <see cref="ProtocolSerializerImplementation{T}"/> class.
            /// </summary>
            static ProtocolSerializerImplementation()
            {
                try
                {
                    var stream = Expression.Parameter(typeof(Stream));
                    var obj = Expression.Parameter(typeof(T));

                    ProtocolSerializerImplementation<T>.Read = Expression.Lambda<Func<Stream, T>>(ProtocolSerializer.ReadValue(stream, typeof(T)), stream).Compile();
                    ProtocolSerializerImplementation<T>.Write = Expression.Lambda<Action<Stream, T>>(ProtocolSerializer.WriteValue(stream, obj), stream, obj).Compile();
                }
                catch (Exception e)
                {
                    ProtocolSerializerImplementation<T>.InitializeError = e;
                }
            }
        }
    }
}