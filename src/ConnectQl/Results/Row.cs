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

namespace ConnectQl.Results
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Extensions;
    using ConnectQl.Internal.Results;

    /// <summary>
    /// The row.
    /// </summary>
    public abstract class Row : Row.IRowImplementation
    {
        /// <summary>
        /// The <see cref="Create{T}"/> method.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly MethodInfo RowCreateMethod = typeof(Row).GetGenericMethod(nameof(Create), typeof(IRowFieldResolver), null, typeof(IEnumerable<KeyValuePair<string, object>>));

        /// <summary>
        /// The values.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        private Row(IRowFieldResolver resolver, object[] values)
        {
            this.Resolver = resolver;
            this.values = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        private Row(IRowFieldResolver resolver)
            : this(resolver, new object[0])
        {
        }

        /// <summary>
        /// The row implementation.
        /// </summary>
        protected interface IRowImplementation
        {
            /// <summary>
            /// Joins the other row to the current row.
            /// </summary>
            /// <typeparam name="TOther">
            /// The type of the other row's unique id.
            /// </typeparam>
            /// <param name="rowBuilder">
            /// The row Builder.
            /// </param>
            /// <param name="other">
            /// The other row.
            /// </param>
            /// <returns>
            /// The joined row, or null when the rows cannot be joined.
            /// </returns>
            Row CombineFrom<TOther>(IRowBuilder rowBuilder, RowImplementation<TOther> other);
        }

        /// <summary>
        /// Gets the column names.
        /// </summary>
        public IReadOnlyList<string> ColumnNames => this.Resolver.Fields;

        /// <summary>
        /// Gets the unique id of the row.
        /// </summary>
        public abstract object UniqueId { get; }

        /// <summary>
        /// Gets the field resolver.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IRowFieldResolver Resolver { get; }

        /// <summary>
        /// Gets the values for debugging purposes.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        protected IDictionary<string, object> Values => this.ToDictionary();

        /// <summary>
        /// Gets or sets the values for the specified field name.
        /// </summary>
        /// <param name="field">
        /// The field name.
        /// </param>
        /// <returns>
        /// The value for the field, or null if the field is not in the row.
        /// </returns>
        public object this[string field]
        {
            get
            {
                var index = this.Resolver.GetIndex(field);

                return index == null || index.Value >= this.values.Length ? null : this.values[index.Value];
            }
        }

        /// <summary>
        /// Converts the row to a dictionary.
        /// </summary>
        /// <returns>
        /// A dictionary containing all fields and their values.
        /// </returns>
        public IDictionary<string, object> ToDictionary()
        {
            return this.ColumnNames.ToDictionary(cn => cn, cn => this[cn], StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Joins the other row to the current row.
        /// </summary>
        /// <typeparam name="TOther">
        /// The type of the other row's unique id.
        /// </typeparam>
        /// <param name="rowBuilder">
        /// The row Builder.
        /// </param>
        /// <param name="other">
        /// The other row.
        /// </param>
        /// <returns>
        /// The joined row, or null when the rows cannot be joined.
        /// </returns>
        Row IRowImplementation.CombineFrom<TOther>(IRowBuilder rowBuilder, RowImplementation<TOther> other)
        {
            return this.CombineFrom(rowBuilder, other);
        }

        /// <summary>
        /// Creates a row.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the unique identifier.
        /// </typeparam>
        /// <param name="fieldResolver">
        /// The data Set.
        /// </param>
        /// <param name="uniqueId">
        /// The unique identifier.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The row.
        /// </returns>
        internal static Row Create<T>(IRowFieldResolver fieldResolver, T uniqueId, IEnumerable<KeyValuePair<string, object>> values)
        {
            if (typeof(T) == typeof(object) && uniqueId.GetType() != typeof(object))
            {
                var parameters = new object[]
                                     {
                                         fieldResolver,
                                         uniqueId,
                                         values,
                                     };

                return (Row)RowCreateMethod.MakeGenericMethod(uniqueId.GetType()).Invoke(null, parameters);
            }

            var rowValues = fieldResolver.GetIndices(values);

            object[] objects;

            if (rowValues.Count > 0)
            {
                objects = new object[rowValues[0].Item1 + 1];

                foreach (var rowValue in rowValues)
                {
                    objects[rowValue.Item1] = rowValue.Item2;
                }
            }
            else
            {
                objects = new object[0];
            }

            return new RowImplementation<T>(fieldResolver, uniqueId, objects);
        }

        /// <summary>
        /// Clones the row in the specified row builder.
        /// </summary>
        /// <param name="rowBuilder">
        /// The builder to clone the row into.
        /// </param>
        /// <returns>
        /// The cloned row.
        /// </returns>
        internal abstract Row Clone(IRowBuilder rowBuilder);

        /// <summary>
        /// Joins the row with this row and returns the result as new row.
        /// </summary>
        /// <param name="set">
        /// The set.
        /// </param>
        /// <param name="row">
        /// The row to join this row with.
        /// </param>
        /// <returns>
        /// A new <see cref="Row"/> containing fields for both the left row and the right row, or null if he join was an inner
        ///     join
        ///     and <paramref name="row"/> was null.
        /// </returns>
        internal abstract Row CombineWith(IRowBuilder set, Row row);

        /// <summary>
        /// Gets the value for the specified field name.
        /// </summary>
        /// <param name="name">
        /// The name of the field to get the value for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value to get.
        /// </typeparam>
        /// <returns>
        /// The value or <c>default(T)</c> if a field is not in the row.
        /// </returns>
        internal T Get<T>(string name)
        {
            return this.GetByIndex<T>(this.Resolver.GetIndex(name));
        }

        /// <summary>
        /// Gets the value for the specified field's internal name.
        /// </summary>
        /// <param name="internalName">
        /// The internal name of the field to get the value for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value to get.
        /// </typeparam>
        /// <returns>
        /// The value or <c>default(T)</c> if a field is not in the row.
        /// </returns>
        internal T GetByInternalName<T>(string internalName)
        {
            return this.GetByIndex<T>(this.Resolver.GetInternalNameIndex(internalName));
        }

        /// <summary>
        /// When implemented in a derived class, joins the other row to the current row.
        /// </summary>
        /// <typeparam name="TOther">
        /// The type of the other row's unique id.
        /// </typeparam>
        /// <param name="rowBuilder">
        /// The row Builder.
        /// </param>
        /// <param name="other">
        /// The other row.
        /// </param>
        /// <returns>
        /// The joined row, or null when the rows cannot be joined.
        /// </returns>
        protected abstract Row CombineFrom<TOther>(IRowBuilder rowBuilder, RowImplementation<TOther> other);

        /// <summary>
        /// Gets a value by index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <returns>
        /// The <typeparamref name="T"/>.
        /// </returns>
        private T GetByIndex<T>(int? index)
        {
            var value = index == null || index.Value >= this.values.Length ? null : this.values[index.Value];

            if (value == null || value is Error)
            {
                return default(T);
            }

            if (typeof(T) == typeof(object))
            {
                return (T)value;
            }

            try
            {
                return typeof(T).IsConstructedGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)
                           ? (T)Convert.ChangeType(value, typeof(T).GenericTypeArguments[0])
                           : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                // Do nothing.
            }

            try
            {
                return (T)value;
            }
            catch
            {
                // Do nothing.
            }

            return default(T);
        }

        /// <summary>
        /// The row implementation.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the unique id.
        /// </typeparam>
        [DebuggerDisplay("{" + nameof(ValuesAsString) + ",nq}")]
        protected class RowImplementation<T> : Row
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Row.RowImplementation{T}"/> class.
            /// </summary>
            /// <param name="resolver">
            /// The field resolver.
            /// </param>
            /// <param name="id">
            /// The unique id.
            /// </param>
            /// <param name="values">
            /// The values.
            /// </param>
            internal RowImplementation(IRowFieldResolver resolver, T id, object[] values)
                : base(resolver, values)
            {
                this.Id = id;
            }

            /// <summary>
            /// Gets the unique id for the row.
            /// </summary>
            public override object UniqueId => this.Id;

            /// <summary>
            /// Gets the unique id.
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public T Id { get; }

            /// <summary>
            /// Gets the string representation of the values.
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string ValuesAsString => string.Join(", ", this.ToDictionary().Select(v => v.Key + ":  " + (v.Value is string ? $"'{v.Value}'" : v.Value ?? "NULL")));

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString() => this.ValuesAsString;

            /// <summary>
            /// Clones the row in the specified row builder.
            /// </summary>
            /// <param name="rowBuilder">
            /// The builder to clone the row into.
            /// </param>
            /// <returns>
            /// The cloned row.
            /// </returns>
            internal override Row Clone(IRowBuilder rowBuilder)
            {
                return rowBuilder.CreateRow(this.Id, this.ToDictionary());
            }

            /// <summary>
            /// Joins the current row with the other row.
            /// </summary>
            /// <param name="rowBuilder">
            /// The row Builder.
            /// </param>
            /// <param name="row">
            /// The row to join.
            /// </param>
            /// <returns>
            /// The joined row, or null if <paramref name="row"/> is null and this is an inner join.
            /// </returns>
            internal override Row CombineWith(IRowBuilder rowBuilder, Row row)
            {
                return ((IRowImplementation)row).CombineFrom(rowBuilder, this);
            }

            /// <summary>
            /// Joins the other row to the current row.
            /// </summary>
            /// <typeparam name="TOther">
            /// The type of the other row's unique id.
            /// </typeparam>
            /// <param name="builder">
            /// The builder.
            /// </param>
            /// <param name="other">
            /// The other row.
            /// </param>
            /// <returns>
            /// The joined row, or null when the rows cannot be joined.
            /// </returns>
            protected override Row CombineFrom<TOther>(IRowBuilder builder, RowImplementation<TOther> other)
            {
                return IdCombiner<T, TOther>.Combine(builder, this, other);
            }

            /// <summary>
            /// The id combiner.
            /// </summary>
            /// <typeparam name="TFirst">
            /// The type of the first unique id.
            /// </typeparam>
            /// <typeparam name="TSecond">
            /// The type of the second unique id.
            /// </typeparam>
            private static class IdCombiner<TFirst, TSecond>
            {
                /// <summary>
                /// The combine.
                /// </summary>
                public static readonly Func<IRowBuilder, RowImplementation<TFirst>, RowImplementation<TSecond>, Row> Combine = CreateCombine();

                /// <summary>
                /// Creates the function that combines two rows into the new <see cref="IRowBuilder"/> for the combination of types.
                /// </summary>
                /// <returns>
                /// The combine function.
                /// </returns>
                private static Func<IRowBuilder, RowImplementation<TFirst>, RowImplementation<TSecond>, Row> CreateCombine()
                {
                    var rowBuilder = Expression.Parameter(typeof(IRowBuilder), "rowBuilder");
                    var first = Expression.Parameter(typeof(RowImplementation<TFirst>), "first");
                    var second = Expression.Parameter(typeof(RowImplementation<TSecond>), "second");
                    var firstId = Expression.Property(first, typeof(RowImplementation<TFirst>).GetRuntimeProperty(nameof(RowImplementation<object>.Id)).GetMethod);
                    var secondId = Expression.Property(second, typeof(RowImplementation<TSecond>).GetRuntimeProperty(nameof(RowImplementation<object>.Id)).GetMethod);
                    var tuple = NewTuple(GetTupleArguments(firstId).Concat(GetTupleArguments(secondId)).ToList());
                    var createRow = typeof(IRowBuilder).GetGenericMethod(nameof(IRowBuilder.CreateRow), null, typeof(IEnumerable<KeyValuePair<string, object>>)).MakeGenericMethod(tuple.Type);
                    var toDictionary = typeof(Row).GetRuntimeMethod(nameof(ToDictionary), new Type[0]);
                    var concat = typeof(Enumerable).GetGenericMethod(nameof(Enumerable.Concat), typeof(IEnumerable<>), typeof(IEnumerable<>)).MakeGenericMethod(typeof(KeyValuePair<string, object>));

                    var values = Expression.Call(
                        concat,
                        Expression.Convert(Expression.Call(first, toDictionary), typeof(IEnumerable<KeyValuePair<string, object>>)),
                        Expression.Convert(Expression.Call(second, toDictionary), typeof(IEnumerable<KeyValuePair<string, object>>)));

                    var lambda = Expression.Lambda<Func<IRowBuilder, RowImplementation<TFirst>, RowImplementation<TSecond>, Row>>(
                        Expression.Call(rowBuilder, createRow, tuple, values),
                        rowBuilder,
                        first,
                        second);

                    return lambda.Compile();
                }

                /// <summary>
                /// Gets the generic type arguments from the parameter.
                /// </summary>
                /// <param name="parameter">
                /// The parameter.
                /// </param>
                /// <returns>
                /// The <see cref="IEnumerable{T}"/>.
                /// </returns>
                private static IEnumerable<Expression> GetTupleArguments(Expression parameter)
                {
                    var type = parameter.Type;

                    if (type.IsConstructedGenericType && type.GetGenericTypeDefinition().Namespace == "System" && type.GetGenericTypeDefinition().Name.StartsWith("Tuple`"))
                    {
                        foreach (var runtimeProperty in type.GetRuntimeProperties().Where(p => p.GetMethod.IsPublic))
                        {
                            foreach (var argument in GetTupleArguments(Expression.Property(parameter, runtimeProperty)))
                            {
                                yield return argument;
                            }
                        }
                    }
                    else
                    {
                        yield return parameter;
                    }
                }

                /// <summary>
                /// Creates a constructor call from the passed in expressions.
                /// </summary>
                /// <param name="expressions">
                /// The expressions.
                /// </param>
                /// <returns>
                /// The <see cref="Expression"/>.
                /// </returns>
                private static NewExpression NewTuple(ICollection<Expression> expressions)
                {
                    if (expressions.Count <= 7)
                    {
                        return Expression.New(
                            Type.GetType($"System.Tuple`{expressions.Count}").MakeGenericType(expressions.Select(e => e.Type).ToArray()).GetTypeInfo().DeclaredConstructors.First(),
                            expressions);
                    }

                    var arguments = expressions.Take(7).ToList();

                    arguments.Add(NewTuple(expressions.Skip(7).ToList()));

                    return Expression.New(
                        Type.GetType("System.Tuple`8").MakeGenericType(arguments.Select(a => a.Type).ToArray()).GetTypeInfo().DeclaredConstructors.First(),
                        arguments.ToArray());
                }
            }
        }
    }
}