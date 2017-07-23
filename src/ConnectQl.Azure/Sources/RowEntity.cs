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

namespace ConnectQl.Azure.Sources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The light entity.
    /// </summary>
    internal class RowEntity : ITableEntity
    {
        /// <summary>
        /// The values.
        /// </summary>
        private readonly Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the entity's current ETag.  Set this value to '*'
        ///     in order to blindly overwrite an entity as part of an update
        ///     operation.
        /// </summary>
        /// <value>
        /// The entity's timestamp.
        /// </value>
        string ITableEntity.ETag
        {
            get
            {
                return this.values.TryGetValue("ETag", out var result) ? result?.ToString() : null;
            }

            set
            {
                this.values["ETag"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity's partition key.
        /// </summary>
        /// <value>
        /// The entity's partition key.
        /// </value>
        public string PartitionKey
        {
            get
            {
                return this.values.TryGetValue("PartitionKey", out var result) ? result?.ToString() : null;
            }

            set
            {
                this.values["PartitionKey"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity's row key.
        /// </summary>
        /// <value>
        /// The entity's row key.
        /// </value>
        public string RowKey
        {
            get
            {
                return this.values.TryGetValue("RowKey", out var result) ? result?.ToString() : null;
            }

            set
            {
                this.values["RowKey"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity's timestamp.
        /// </summary>
        /// <value>
        /// The entity's timestamp. The property is populated by the Windows Azure Table Service.
        /// </value>
        DateTimeOffset ITableEntity.Timestamp
        {
            get
            {
                return this.values.TryGetValue("Timestamp", out var result) ? (DateTimeOffset)result : DateTimeOffset.Now;
            }

            set
            {
                this.values["Timestamp"] = value;
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object this[string field]
        {
            get
            {
                return this.values.TryGetValue(field, out var result) ? result : null;
            }
        }

        /// <summary>
        /// Populates the entity's properties from the <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityProperty"/>
        ///     data values in the <paramref name="properties"/> dictionary.
        /// </summary>
        /// <param name="properties">
        /// The dictionary of string property names to
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityProperty"/> data values to deserialize and store in this
        ///     table entity instance.
        /// </param>
        /// <param name="operationContext">
        /// An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext"/> object that
        ///     represents the context for the current operation.
        /// </param>
        void ITableEntity.ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            foreach (var property in properties)
            {
                this.values[property.Key] = property.Value?.PropertyAsObject;
            }
        }

        /// <summary>
        /// Serializes the <see cref="T:System.Collections.Generic.IDictionary`2"/> of property names mapped to
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityProperty"/> data values from the entity instance.
        /// </summary>
        /// <param name="operationContext">
        /// An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext"/> object that
        ///     represents the context for the current operation.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IDictionary`2"/> object of property names to
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityProperty"/> data typed values created by serializing this
        ///     table entity instance.
        /// </returns>
        IDictionary<string, EntityProperty> ITableEntity.WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the values from this row.
        /// </summary>
        /// <param name="fieldNames">
        /// The field names to return. When this is null, all fields are returned.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<string, object>> GetValues(IEnumerable<string> fieldNames)
        {
            return fieldNames == null ? this.values : this.values.Join(fieldNames, v => v.Key, f => f, (v, f) => v);
        }
    }
}