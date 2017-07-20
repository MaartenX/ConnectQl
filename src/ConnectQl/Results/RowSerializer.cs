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

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Internal.Results;

    /// <summary>
    /// The row serializer.
    /// </summary>
    public class RowSerializer : ITransform<Row>
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="RowSerializer"/> class from being created.
        /// </summary>
        private RowSerializer()
        {
        }

        /// <summary>
        /// Gets the default row serializer.
        /// </summary>
        public static ITransform<Row> Default { get; } = new RowSerializer();

        /// <summary>
        ///     Gets the target type values will be transformed to.
        /// </summary>
        public Type TargetType { get; } = typeof(SerializableRow);

        /// <summary>
        ///     Creates a transformation context.
        ///     This context will be used in all calls to Serialize and Deserialize.
        /// </summary>
        /// <returns>
        ///     The context that will be disposed when the transformation is no longer needed.
        /// </returns>
        public IDisposable CreateContext()
        {
            return new SerializerContext();
        }

        /// <summary>
        ///     Transforms a serializable object to a value.
        /// </summary>
        /// <param name="context">
        ///     The context in which this serializable object is transformed.
        /// </param>
        /// <param name="value">
        ///     The serializable object to transform.
        /// </param>
        /// <returns>
        ///     The value.
        /// </returns>
        public Row Deserialize(IDisposable context, object value)
        {
            var serializableRow = (SerializableRow)value;

            return ((SerializerContext)context).GetResolver(serializableRow.BuilderId).DeserializeRow(serializableRow.Id, serializableRow.Values);
        }

        /// <summary>
        ///     Transforms the value to the serializable object.
        /// </summary>
        /// <param name="context">
        ///     The context in which this value is transformed.
        /// </param>
        /// <param name="value">
        ///     The value to transform.
        /// </param>
        /// <returns>
        ///     A serializable version of the value.
        /// </returns>
        public object Serialize(IDisposable context, Row value)
        {
            var serialized = value.Resolver.SerializeRow(value);

            return new SerializableRow
                       {
                           BuilderId = ((SerializerContext)context).GetResolverId(value.Resolver),
                           Id = serialized.Item1,
                           Values = serialized.Item2,
                       };
        }

        /// <summary>
        /// The serializable row.
        /// </summary>
        private class SerializableRow
        {
            /// <summary>
            /// Gets or sets the builder id.
            /// </summary>
            public int BuilderId { get; set; }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public object Id { get; set; }

            /// <summary>
            /// Gets or sets the values.
            /// </summary>
            public IDictionary<string, object> Values { get; set; }
        }

        /// <summary>
        /// The serializer context.
        /// </summary>
        private class SerializerContext : IDisposable
        {
            /// <summary>
            /// The resolvers by id.
            /// </summary>
            private readonly Dictionary<int, IRowFieldResolver> resolversById = new Dictionary<int, IRowFieldResolver>();

            /// <summary>
            /// The ids by resolver.
            /// </summary>
            private readonly Dictionary<IRowFieldResolver, int> idsByResolver = new Dictionary<IRowFieldResolver, int>(new ReferenceEqualityComparer<IRowBuilder>());

            /// <summary>
            /// The current id.
            /// </summary>
            private int currentId;

            /// <summary>
            /// Disposes the context.
            /// </summary>
            public void Dispose()
            {
                this.idsByResolver.Clear();
                this.resolversById.Clear();
            }

            /// <summary>
            /// Gets the resolver by id.
            /// </summary>
            /// <param name="id">
            /// The id.
            /// </param>
            /// <returns>
            /// The <see cref="IRowFieldResolver"/>.
            /// </returns>
            internal IRowFieldResolver GetResolver(int id)
            {
                return this.resolversById[id];
            }

            /// <summary>
            /// Gets the id for a resolver.
            /// </summary>
            /// <param name="resolver">
            /// The resolver.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            internal int GetResolverId(IRowFieldResolver resolver)
            {
                if (this.idsByResolver.TryGetValue(resolver, out int id))
                {
                    return id;
                }

                id = ++this.currentId;

                this.idsByResolver[resolver] = id;
                this.resolversById[id] = resolver;

                return id;
            }
        }
    }
}