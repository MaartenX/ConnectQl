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
namespace ConnectQl.Internal.DataSources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerablePolicies;
    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Interfaces;
    using ConnectQl.Internal.Comparers;
    using ConnectQl.Results;

    /// <summary>
    /// The DCAST function.
    /// </summary>
    public enum DCastFunction
    {
        /// <summary>
        /// Gets the first value in case of duplicates.
        /// </summary>
        First,

        /// <summary>
        /// Gets the last value in case of duplicates
        /// </summary>
        Last,

        /// <summary>
        /// Gets the minimum value in case of duplicates
        /// </summary>
        Min,

        /// <summary>
        /// Gets the maximum value in case of duplicates
        /// </summary>
        Max,

        /// <summary>
        /// Gets the average value in case of duplicates
        /// </summary>
        Avg,

        /// <summary>
        /// Concatenates all values.
        /// </summary>
        Concat,
    }

    /// <summary>
    /// The d cast.
    /// </summary>
    public class DCast : IDataSource
    {
        /// <summary>
        /// The column name.
        /// </summary>
        private readonly string columnName;

        /// <summary>
        /// The column value.
        /// </summary>
        private readonly string columnValue;

        private readonly DCastFunction function;

        /// <summary>
        /// The source.
        /// </summary>
        private readonly IAsyncEnumerable<Row> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="DCast"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <param name="columnValue">
        /// The column value.
        /// </param>
        /// <param name="function">
        /// The function.
        /// </param>
        public DCast(IAsyncEnumerable<Row> source, string columnName, string columnValue, DCastFunction function)
        {
            this.source = source;
            this.columnName = columnName;
            this.columnValue = columnValue;
            this.function = function;
        }

        /// <summary>
        ///     Retrieves the data from the source as an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="rowBuilder">
        ///     The row builder.
        /// </param>
        /// <param name="query">
        ///     The query expression. Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     A task returning the data set.
        /// </returns>
        public IAsyncEnumerable<Row> GetRows(IExecutionContext context, IRowBuilder rowBuilder, IQuery query)
        {
            return context.CreateAsyncEnumerable(() => this.GetRowsAsync(context, rowBuilder));
        }

        /// <summary>
        /// Converts the value to a double.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Double}"/>.
        /// </returns>
        private static double? ToDouble(object value)
        {
            if (value is double)
            {
                return (double)value;
            }

            if (value is float || value is long || value is int || value is short || value is byte || value is ulong || value is uint || value is ushort)
            {
                return Convert.ToDouble(value);
            }

            if (value is string str && double.TryParse(str, out double result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        ///     Gets the rows asynchronously.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="rowBuilder">
        ///     The row builder.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task<IAsyncEnumerable<Row>> GetRowsAsync(IExecutionContext context, IRowBuilder rowBuilder)
        {
            var rows = await this.source.MaterializeAsync().ConfigureAwait(false);
            var first = await rows.FirstOrDefaultAsync().ConfigureAwait(false);

            if (first == null)
            {
                return context.CreateEmptyAsyncEnumerable<Row>();
            }

            var sortColumns = first.ColumnNames.Where(c => c != this.columnName && c != this.columnValue).ToArray();
            var groups = rows.GroupBy(row => sortColumns.Select(c => row[c]).ToArray(), ArrayOfObjectComparer.Default);

            return groups.Select(
                async group =>
                    {
                        var values = sortColumns
                            .Zip(group.Key, (name, value) => new KeyValuePair<string, object>(name, value))
                            .ToDictionary(kv => kv.Key, kv => kv.Value);

                        var counts = values.ToDictionary(kv => kv.Key, kv => 0);

                        object id = null;

                        await group.ForEachAsync(
                            row =>
                                {
                                    id = row.UniqueId;
                                    var name = row[this.columnName] as string;
                                    var value = row[this.columnValue];

                                    if (name != null)
                                    {
                                        switch (this.function)
                                        {
                                            case DCastFunction.Concat:
                                                if (values.TryGetValue(name, out object existingValue))
                                                {
                                                    values[name] = $"{(string)existingValue}, {value}";
                                                }
                                                else
                                                {
                                                    values[name] = value;
                                                }
                                                break;
                                            case DCastFunction.First:
                                                if (!values.ContainsKey(name))
                                                {
                                                    values[name] = value;
                                                }

                                                break;
                                            case DCastFunction.Last:
                                                values[name] = value;
                                                break;
                                            case DCastFunction.Min:

                                                if (values.TryGetValue(name, out existingValue))
                                                {
                                                    if (Comparer<object>.Default.Compare(existingValue, value) > 0)
                                                    {
                                                        values[name] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    values[name] = value;
                                                }

                                                break;
                                            case DCastFunction.Max:

                                                if (values.TryGetValue(name, out existingValue))
                                                {
                                                    if (Comparer<object>.Default.Compare(existingValue, value) < 0)
                                                    {
                                                        values[name] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    values[name] = value;
                                                }

                                                break;

                                            case DCastFunction.Avg:

                                                if (values.TryGetValue(name, out existingValue))
                                                {
                                                    var valueDouble = DCast.ToDouble(value);
                                                    var existingDouble = DCast.ToDouble(existingValue);

                                                    if (valueDouble != null && existingDouble != null)
                                                    {
                                                        values[name] = valueDouble + existingDouble;
                                                    }
                                                }
                                                else
                                                {
                                                    values[name] = DCast.ToDouble(value) ?? value;
                                                }

                                                break;
                                        }

                                        values[name] = row[this.columnValue];
                                        counts[name] = counts.TryGetValue(name, out int count) ? count + 1 : 1;
                                    }
                                });

                        if (this.function == DCastFunction.Avg)
                        {
                            foreach (var key in values.Keys.Except(sortColumns))
                            {
                                if (values[key] is double)
                                {
                                    values[key] = ((double)values[key]) / counts[key];
                                }
                            }
                        }

                        return rowBuilder.CreateRow(id, values);
                    });
        }
    }
}