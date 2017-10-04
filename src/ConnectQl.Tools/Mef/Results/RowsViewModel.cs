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

namespace ConnectQl.Tools.Mef.Results
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using AsyncEnumerablePolicies;
    using AsyncEnumerables;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    /// <summary>
    /// The query result view model.
    /// </summary>
    public class RowsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RowsViewModel" /> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        public RowsViewModel(IAsyncEnumerable<Row> rows)
        {
            this.InitializeAsync(rows);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the rows.
        /// </summary>
        public DataTable Rows { get; private set; }

        /// <summary>
        /// Initializes the observable collection.
        /// </summary>
        /// <param name="rows">
        /// The rows to add to the view model.
        /// </param>
        public async void InitializeAsync([NotNull] IAsyncEnumerable<Row> rows)
        {
            var table = new DataTable();

            var types = new Dictionary<string, HashSet<Type>>();

            var rowCollection = await new InMemoryPolicy().MaterializeAsync(rows.Select(row =>
            {
                foreach (var column in row.ColumnNames)
                {
                    var value = row[column];

                    if (value != null)
                    {
                        var typeSet = types.TryGetValue(column, out var hashSet) ? hashSet : (hashSet = types[column] = new HashSet<Type>());
                        typeSet.Add(value.GetType());
                    }
                }

                return row;
            }));

            var first = await rowCollection.FirstOrDefaultAsync();

            if (first != null)
            {
                var columns = first.ColumnNames
                                .Select(c => new DataColumn
                                {
                                    ColumnName = c,
                                    DataType = types.TryGetValue(c, out var hashSet) && hashSet.Count == 1 ? hashSet.First() : typeof(object)
                                })
                                .ToArray();

                table.Columns.AddRange(columns);

                await rowCollection.ForEachAsync(row =>
                {
                    var tableRow = table.NewRow();

                    foreach (var column in row.ColumnNames)
                    {
                        var value = row[column];

                        if (value != null)
                        {
                            tableRow[column] = value;
                        }
                    }

                    table.Rows.Add(tableRow);
                });
            }

            this.Rows = table;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Rows)));
        }
    }
}
