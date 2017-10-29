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
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Expressions;
    using ConnectQl.Expressions.Visitors;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The table query source.
    /// </summary>
    internal class TableDataSource : IDataSource, IDataTarget, IDataSourceFilterSupport, IDataSourceOrderBySupport, IDescriptableDataSource
    {
        /// <summary>
        /// Converts an <see cref="ExpressionType"/> to <see cref="string"/>.
        /// </summary>
        private static readonly Dictionary<ExpressionType, string> ExpressionTypeToString =
            new Dictionary<ExpressionType, string>
                {
                    {
                        ExpressionType.GreaterThan, "gt"
                    },
                    {
                        ExpressionType.GreaterThanOrEqual, "ge"
                    },
                    {
                        ExpressionType.LessThan, "lt"
                    },
                    {
                        ExpressionType.LessThanOrEqual, "le"
                    },
                    {
                        ExpressionType.AndAlso, "and"
                    },
                    {
                        ExpressionType.And, "and"
                    },
                    {
                        ExpressionType.OrElse, "or"
                    },
                    {
                        ExpressionType.Or, "or"
                    },
                    {
                        ExpressionType.Equal, "eq"
                    },
                    {
                        ExpressionType.NotEqual, "neq"
                    },
                    {
                        ExpressionType.Add, "+"
                    },
                    {
                        ExpressionType.Subtract, "-"
                    }
                };

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataSource"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public TableDataSource(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataSource"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public TableDataSource(string name, string connectionString)
        {
            this.name = name;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the descriptor for this data source.
        /// </summary>
        /// <param name="sourceAlias">
        /// The source alias.
        /// </param>
        /// <param name="context">
        ///     The execution context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string sourceAlias, IExecutionContext context)
        {
            var maxRowsToScan = context.MaxRowsToScan;
            var table = this.GetTable(context);

            TableContinuationToken token = null;

            var rows = new List<RowEntity>();
            var lines = 0;
            var fields = new HashSet<string>();
            var types = new Dictionary<string, Type>();

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery(), CreateRowEntity, token);

                token = segment.ContinuationToken;

                foreach (var rowEntity in segment.Results.Select(r => r.GetValues(null)))
                {
                    if (++lines > maxRowsToScan)
                    {
                        token = null;
                        break;
                    }

                    foreach (var kv in rowEntity)
                    {
                        var fieldType = kv.Value?.GetType() ?? typeof(object);
                        if (fields.Add(kv.Key))
                        {
                            types[kv.Key] = fieldType;
                        }
                        else
                        {
                            var type = types[kv.Key];
                            if (type != fieldType && type != typeof(object))
                            {
                                types[kv.Key] = typeof(object);
                            }
                        }
                    }
                }

                rows.AddRange(segment.Results);
            }
            while (token != null);

            return Descriptor.ForDataSource(sourceAlias, fields.Select(f => Descriptor.ForColumn(f, types[f])));
        }

        /// <summary>
        /// Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowBuilder">
        /// The row builder.
        /// </param>
        /// <param name="query">
        /// The query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        /// A task returning the data set.
        /// </returns>
        public IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
        {
            var table = this.GetTable(context);

            var tableQuery =
                new TableQuery().Select(query.RetrieveAllFields ? null : query.Fields.Select(f => f).ToList())
                    .Where(ToTableQuery(query.GetFilter(context)))
                    .Take(query.Count);

            var result = context.CreateAsyncEnumerable(
                async (GetDataState state) =>
                    {
                        if (state.Done)
                        {
                            return null;
                        }

                        var segment = await table.ExecuteQuerySegmentedAsync(tableQuery, CreateRowEntity, state.Token);

                        state.Done = segment.ContinuationToken == null;
                        state.Token = segment.ContinuationToken;

                        return segment.Results.Count != 0
                                   ? segment.Select(
                                       row =>
                                           rowBuilder.CreateRow(
                                               Tuple.Create(row.PartitionKey, row.RowKey),
                                               row.GetValues(tableQuery.SelectColumns)))
                                   : null;
                    });

            result.BeforeFirstElement(
                () =>
                    context.Logger.Verbose(
                        tableQuery.FilterString == null
                            ? $"Retrieving all records from table '{table.Name}'."
                            : $"Retrieving records from table '{table.Name}', query : {tableQuery.FilterString}."));

            result.AfterLastElement(
                count => context.Logger.Verbose($"Retrieved {count} records from table '{table.Name}'."));

            return result;
        }

        /// <summary>
        /// Checks if the source supports the expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to check for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the expression is supported, false otherwise.
        /// </returns>
        public bool SupportsExpression(BinaryExpression expression)
        {
            var field = (expression.Left as FieldExpression)?.FieldName;

            return (field == "RowKey" || field == "PartitionKey") && !expression.Right.GetFields().Any();
        }

        /// <summary>
        /// Checks if the data source supports the ORDER BY expressions.
        /// </summary>
        /// <param name="orderBy">
        /// The expression to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the expressions are supported, false otherwise.
        /// </returns>
        public bool SupportsOrderBy(IEnumerable<IOrderByExpression> orderBy)
        {
            var sortOrders = orderBy;

            return !sortOrders.Any(
                       s =>
                           {
                               var field = s.Expression as FieldExpression;

                               return field == null
                                      || (!field.FieldName.Equals("RowKey", StringComparison.OrdinalIgnoreCase) && !field.FieldName.Equals("PartitionKey", StringComparison.OrdinalIgnoreCase))
                                      || s.Ascending == false;
                           });
        }

        /// <summary>
        /// Writes the rowsToWrite to the specified target.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rowsToWrite">
        /// The rowsToWrite.
        /// </param>
        /// <param name="upsert">
        /// True to also update records, false to insert.
        /// </param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task"/>.
        /// </returns>
        public async Task<long> WriteRowsAsync(IExecutionContext context, IAsyncEnumerable<Row> rowsToWrite, bool upsert)
        {
            var action = upsert
                             ? (operation, entity) => operation.InsertOrReplace(entity)
                             : (Action<TableBatchOperation, ITableEntity>)((operation, entity) => operation.Insert(entity));

            long duplicates = 0;

            Func<IEnumerable<ITableEntity>, IEnumerable<ITableEntity>> detectDuplicates =
                batch => batch.GroupBy(o => new
                                                {
                                                    o.PartitionKey,
                                                    o.RowKey
                                                }).Select(
                    g =>
                        {
                            ITableEntity result = null;
                            var groupCount = 0;

                            foreach (var row in g)
                            {
                                groupCount++;
                                result = row;
                            }

                            duplicates += groupCount - 1;

                            return result;
                        });

            var operations =
                rowsToWrite.Select(ToEntity)
                    .Batch(100, row => row.PartitionKey)
                    .Select(
                        async batch =>
                            CreateTableBatchOperation(
                                detectDuplicates(await batch.ToArrayAsync().ConfigureAwait(false)),
                                action));

            var account =
                CloudStorageAccount.Parse(
                    this.connectionString ?? context.GetDefault("CONNECTIONSTRING", this).ToString());
            var table = account.CreateCloudTableClient().GetTableReference(this.name);
            var tableNotCreatedYet = true;

            long count = 0;

            await operations.Batch(3).ForEachAsync(
                async ops =>
                    {
                        var operationArray = await ops.ToArrayAsync().ConfigureAwait(false);

                        if (tableNotCreatedYet)
                        {
                            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

                            tableNotCreatedYet = false;
                        }

                        Func<TableBatchOperation, Task<long>> createBatch = async operation =>
                            {
                                var tries = 3;

                                while (tries-- > 0)
                                {
                                    try
                                    {
                                        await table.ExecuteBatchAsync(operation).ConfigureAwait(false);

                                        return operation.Count;
                                    }
                                    catch (StorageException)
                                    {
                                        if (tries == 0)
                                        {
                                            throw;
                                        }
                                    }
                                }

                                return 0;
                            };

                        count += (await Task.WhenAll(operationArray.Select(createBatch)).ConfigureAwait(false)).Sum();
                    }).ConfigureAwait(false);

            if (duplicates > 0)
            {
                context.Logger.Warning(
                    "Found records with identical PartitionKey/RowKey combinations. This can impact performance.");
            }

            return count;
        }

        /// <summary>
        /// Creates a row entity.
        /// </summary>
        /// <param name="partitionkey">
        /// The partition key.
        /// </param>
        /// <param name="rowkey">
        /// The row key.
        /// </param>
        /// <param name="timestamp">
        /// The time stamp.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="etag">
        /// The E-tag.
        /// </param>
        /// <returns>
        /// The <see cref="RowEntity"/>.
        /// </returns>
        private static RowEntity CreateRowEntity(
            string partitionkey,
            string rowkey,
            DateTimeOffset timestamp,
            IDictionary<string, EntityProperty> properties,
            string etag)
        {
            var result = new RowEntity
                             {
                                 PartitionKey = partitionkey, RowKey = rowkey
                             };

            var tableEntity = (ITableEntity)result;

            tableEntity.ETag = etag;
            tableEntity.ReadEntity(properties, null);

            return result;
        }

        /// <summary>
        /// Creates a table batch operation.
        /// </summary>
        /// <param name="batch">
        /// The batch.
        /// </param>
        /// <param name="entityAction">
        /// The entity action.
        /// </param>
        /// <returns>
        /// The <see cref="TableBatchOperation"/>.
        /// </returns>
        private static TableBatchOperation CreateTableBatchOperation(
            IEnumerable<ITableEntity> batch,
            Action<TableBatchOperation, ITableEntity> entityAction)
        {
            return batch.Aggregate(
                new TableBatchOperation(),
                (operation, entity) =>
                    {
                        entityAction(operation, entity);
                        return operation;
                    });
        }

        /// <summary>
        /// Converts the row to an entity.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The row as a table entity.
        /// </returns>
        private static ITableEntity ToEntity(Row row)
        {
            var dte = new DynamicTableEntity();

            foreach (var value in row.ToDictionary())
            {
                if (value.Key.Equals("PartitionKey", StringComparison.OrdinalIgnoreCase))
                {
                    dte.PartitionKey = value.Value?.ToString() ?? string.Empty;
                }
                else if (value.Key.Equals("RowKey", StringComparison.OrdinalIgnoreCase))
                {
                    dte.RowKey = value.Value?.ToString() ?? string.Empty;
                }
                else if (value.Key.Equals("Timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    var timestamp = row[value.Key];
                    dte.Timestamp = timestamp is DateTime
                                        ? new DateTimeOffset((DateTime)timestamp)
                                        : (DateTimeOffset)timestamp;
                }
                else
                {
                    dte.Properties[value.Key] = EntityProperty.CreateEntityPropertyFromObject(value.Value);
                }
            }

            if (dte.PartitionKey == null)
            {
                throw new InvalidOperationException("Missing partition key.");
            }

            if (dte.RowKey == null)
            {
                throw new InvalidOperationException("Missing row key.");
            }

            return dte;
        }

        /// <summary>
        /// The to table query.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throw when trying to filter on anything else than a row key or partition key.
        /// </exception>
        private static string ToTableQuery(Expression filter)
        {
            if (filter == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            // ReSharper disable once AccessToModifiedClosure
            new GenericVisitor
                    {
                        (FieldExpression node) =>
                            {
                                sb.Append(node.FieldName);
                                return node;
                            },
                        (GenericVisitor visitor, BinaryExpression node) =>
                            {
                                sb.Append("(");

                                visitor.Visit(node.Left);

                                sb.Append($" {ExpressionTypeToString[node.NodeType]} ");

                                visitor.Visit(node.Right);

                                sb.Append(")");

                                return node;
                            },
                        (ConstantExpression node) =>
                            {
                                sb.Append($"'{node.Value}'");

                                return node;
                            }
                    }

                // .Default(node => { throw new InvalidOperationException($"Could not translate query part {node}."); })
                .Visit(filter);

            return sb.ToString();
        }

        /// <summary>
        /// The get table.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="CloudTable"/>.
        /// </returns>
        private CloudTable GetTable(IExecutionContext context)
        {
            var table =
                CloudStorageAccount.Parse(
                        this.connectionString ?? context.GetDefault("CONNECTIONSTRING", this).ToString())
                    .CreateCloudTableClient()
                    .GetTableReference(this.name);
            return table;
        }

        /// <summary>
        /// Stores the state for the enumerator.
        /// </summary>
        private class GetDataState
        {
            /// <summary>
            /// Gets or sets a value indicating whether done.
            /// </summary>
            public bool Done { get; set; }

            /// <summary>
            /// Gets or sets the token.
            /// </summary>
            public TableContinuationToken Token { get; set; }
        }
    }
}