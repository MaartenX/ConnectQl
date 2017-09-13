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

namespace ConnectQl.Excel.FileFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;
    using OfficeOpenXml;

    /// <summary>
    ///     The Excel file format.
    /// </summary>
    public class ExcelFileFormat : IFileFormat, IDescriptableFileFormat, IOverrideEncoding
    {
        /// <summary>
        ///     The packages.
        /// </summary>
        private readonly Dictionary<Stream, Package> packages = new Dictionary<Stream, Package>();

        /// <summary>
        ///     Gets the encoding.
        /// </summary>
        Encoding IOverrideEncoding.Encoding { get; } = new UTF8Encoding(false);

        /// <summary>
        /// Gets a value indicating whether the collection of rows should be materialized when
        /// calling this writer.
        /// When all columns are needed in the header (e.g. for CSV or Excel), you should return <c>true</c>
        /// here. Other formats that use the columns per object (like JSON) can return <c>false</c>.
        /// </summary>
        public bool ShouldMaterialize { get; } = true;

        /// <summary>
        ///     Checks if the file reader can read this file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="fileName">
        ///     The file name.
        /// </param>
        /// <param name="firstBytes">
        ///     The first bytes of the file.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this reader can read the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanReadThisFile(IFileFormatExecutionContext context, string fileName, byte[] firstBytes)
        {
            return fileName.EndsWith(".xlsx");
        }

        /// <summary>
        ///     Checks if the file writer can write this file.
        /// </summary>
        /// <param name="fileName">
        ///     The file name.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this reader can write the file, <c>false</c> otherwise.
        /// </returns>
        public bool CanWriteThisFile(string fileName)
        {
            return fileName.EndsWith(".xlsx");
        }

        /// <summary>
        ///     Gets the descriptor for this data source.
        /// </summary>
        /// <param name="alias">
        ///     The alias of the data source.
        /// </param>
        /// <param name="context">
        ///     The execution context.
        /// </param>
        /// <param name="reader">
        ///     The reader.
        /// </param>
        /// <returns>
        ///     The <see cref="System.Threading.Tasks.Task" />.
        /// </returns>
        public Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(string alias, IFileFormatExecutionContext context, StreamReader reader)
        {
            using (var package = new ExcelPackage(reader.BaseStream))
            {
                var sheet = package.Workbook.Worksheets.FirstOrDefault();

                if (sheet == null)
                {
                    return Task.FromResult(Descriptor.DynamicDataSource(alias));
                }

                var headers = Enumerable.Range(1, sheet.Dimension.End.Column).Select(col => sheet.Cells[1, col].Value?.ToString()).TakeWhile(header => header != null).ToArray();

                return Task.FromResult(Descriptor.ForDataSource(alias, headers.Select(h => Descriptor.ForColumn(h, typeof(object)))));
            }
        }

        /// <summary>
        ///     Reads objects from the stream.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="rowBuilder">
        ///     The row builder.
        /// </param>
        /// <param name="reader">
        ///     The stream.
        /// </param>
        /// <param name="fields">
        ///     The fields, or <c>null</c> to retrieve all fields.
        /// </param>
        /// <returns>
        ///     The rows.
        /// </returns>
        public IEnumerable<Row> Read(IFileFormatExecutionContext context, IRowBuilder rowBuilder, StreamReader reader, HashSet<string> fields)
        {
            using (var package = new ExcelPackage(reader.BaseStream))
            {
                var sheet = package.Workbook.Worksheets.FirstOrDefault();

                if (sheet == null)
                {
                    yield break;
                }

                long idx = 0;
                var headers = Enumerable.Range(1, sheet.Dimension.End.Column).Select(col => sheet.Cells[1, col].Value?.ToString()).TakeWhile(header => header != null).ToArray();

                foreach (var range in Enumerable.Range(2, sheet.Dimension.End.Row - 1).Select(row => sheet.Cells[row, 1, row, headers.Length].Select(cell => cell.Value)))
                {
                    yield return rowBuilder.CreateRow(idx++, headers.Zip(range, (header, value) => new KeyValuePair<string, object>(header, value)));
                }
            }
        }

        /// <summary>
        ///     Writes the footer to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        public void WriteFooter(IFileFormatExecutionContext context, StreamWriter writer)
        {
            var package = this.packages[writer.BaseStream];
            var sheet = package.ExcelPackage.Workbook.Worksheets[package.Sheet];

            if (sheet.Dimension != null)
            {
                sheet.Cells[1, 1, Math.Min(100, sheet.Dimension.Rows), sheet.Dimension.Columns].AutoFitColumns();
            }

            this.packages[writer.BaseStream].ExcelPackage.SaveAs(writer.BaseStream);
            this.packages[writer.BaseStream].ExcelPackage.Dispose();
            this.packages.Remove(writer.BaseStream);

            writer.BaseStream.Flush();
        }

        /// <summary>
        ///     Writes the header to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        /// <param name="fields">
        ///     The fields.
        /// </param>
        public void WriteHeader(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<string> fields)
        {
            var package = this.packages[writer.BaseStream] = new Package
                                                                 {
                                                                     ExcelPackage = new ExcelPackage(),
                                                                     Sheet = "Data",
                                                                     Header = fields.ToArray()
                                                                 };

            var sheet = package.ExcelPackage.Workbook.Worksheets.Add(package.Sheet);
            var idx = 0;

            foreach (var field in package.Header)
            {
                sheet.Cells[1, ++idx].Value = field;
                sheet.Cells[1, idx].Style.Font.Bold = true;
            }
        }

        /// <summary>
        ///     Writes rows to the file.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="writer">
        ///     The stream.
        /// </param>
        /// <param name="rows">
        ///     The rows to write.
        /// </param>
        /// <param name="upsert">
        ///     True to UPSERT, false to INSERT.
        /// </param>
        /// <returns>
        ///     The number of rows that were written.
        /// </returns>
        public long WriteRows(IFileFormatExecutionContext context, StreamWriter writer, IEnumerable<Row> rows, bool upsert)
        {
            var package = this.packages[writer.BaseStream];
            var sheet = package.ExcelPackage.Workbook.Worksheets[package.Sheet];
            var count = 0;

            foreach (var row in rows)
            {
                package.RecordPosition++;
                count++;

                if (package.RecordPosition == 1048576)
                {
                    sheet.Cells[1, 1, Math.Min(100, sheet.Dimension.Rows), sheet.Dimension.Columns].AutoFitColumns();

                    if (package.ExcelPackage.Workbook.Worksheets.Count == 1)
                    {
                        context.Logger.Warning("More than 1048575 records found, exporting to multiple sheets.");
                    }

                    package.Sheet = $"Data ({package.ExcelPackage.Workbook.Worksheets.Count + 1})";
                    package.ExcelPackage.Workbook.Worksheets.Add(package.Sheet);
                    package.RecordPosition = 1;

                    sheet = package.ExcelPackage.Workbook.Worksheets[package.Sheet];

                    var idx = 0;

                    foreach (var field in package.Header)
                    {
                        sheet.Cells[1, ++idx].Value = field;
                        sheet.Cells[1, idx].Style.Font.Bold = true;
                    }
                }

                var col = 0;
                foreach (var column in row.ColumnNames)
                {
                    var value = row[column];
                    var cell = sheet.Cells[package.RecordPosition + 1, ++col];

                    cell.Value = row[column];

                    if (value is string)
                    {
                        cell.Style.Numberformat.Format = "@";
                    }
                    else if (value is DateTime)
                    {
                        cell.Style.Numberformat.Format = "m/d/yy h:mm";
                    }
                    else if (value is Error)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// The package information.
        /// </summary>
        private class Package
        {
            /// <summary>
            /// Gets or sets the excel package.
            /// </summary>
            public ExcelPackage ExcelPackage { get; set; }

            /// <summary>
            /// Gets or sets the record position.
            /// </summary>
            public int RecordPosition { get; set; }

            /// <summary>
            /// Gets or sets the sheet.
            /// </summary>
            public string Sheet { get; set; }

            /// <summary>
            /// Gets or sets the headers.
            /// </summary>
            public string[] Header { get; set; }
        }
    }
}