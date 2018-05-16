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

namespace ConnectQl.Excel.Tests
{
    using System.IO;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.DataSources;
    using ConnectQl.Interfaces;
    using Moq;
    using OfficeOpenXml;
    using Xunit;

    /// <summary>
    /// Tests for the Excel file format.
    /// </summary>
    public class ExcelFileFormatTests
    {
        /// <summary>
        /// INSERT should create an Excel file.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact(DisplayName = "INSERT should create an Excel file.")]
        public async Task InsertShouldCreateFile()
        {
            var resolver = new Mock<IPluginResolver>();
            var uriResolver = new Mock<IUriResolver>();

            using (var target = new MemoryStream())
            {
                resolver.Setup(pr => pr.EnumerateAvailablePlugins()).Returns(new[] { new Plugin() });
                uriResolver.Setup(ur => ur.ResolveToStream("file.xlsx", UriResolveMode.Write)).Returns(Task.FromResult<Stream>(target));

                var context = new ConnectQlContext(resolver.Object) { UriResolver = uriResolver.Object };

                var r = await context.ExecuteAsync("IMPORT PLUGIN 'Excel' INSERT INTO FILE('file.xlsx') SELECT * FROM SPLIT('file.xlsx', '.') f");

                Assert.Equal(1, r.QueryResults.Count);

                using (var ms = new MemoryStream(target.ToArray()))
                using (var sheets = new ExcelPackage(ms).Workbook.Worksheets)
                {
                    Assert.Equal(1, sheets.Count);
                    Assert.Equal("Item", sheets[1].Cells[1, 1].Value);
                    Assert.Equal("file", sheets[1].Cells[2, 1].Value);
                    Assert.Equal("xlsx", sheets[1].Cells[3, 1].Value);
                }
            }
        }

        /// <summary>
        /// TheSELECT should return a record set.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="expectedRecordCount">
        /// The expected record count.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "SELECT should return a record set.")]
        [InlineData("SELECT * FROM FILE('file.xlsx') f", 4)]
        [InlineData("SELECT * FROM FILE('file.xlsx') f WHERE f.id < 3", 2)]
        [InlineData("SELECT * FROM FILE('file.xlsx') f WHERE f.value <> 4", 3)]
        [InlineData("SELECT * FROM FILE('file.xlsx') f WHERE f.value = f.id", 1)]
        [InlineData("SELECT * FROM FILE('file.xlsx') f WHERE f.value <> f.id", 3)]
        public async Task SelectShouldReturnRecords(string query, int expectedRecordCount)
        {
            var resolver = new Mock<IPluginResolver>();
            var uriResolver = new Mock<IUriResolver>();

            using (var target = new MemoryStream())
            {
                var package = new ExcelPackage();
                var sheet = package.Workbook.Worksheets.Add("Data");

                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[2, 1].Value = "1";
                sheet.Cells[3, 1].Value = "2";
                sheet.Cells[4, 1].Value = "3";
                sheet.Cells[5, 1].Value = "4";

                sheet.Cells[1, 2].Value = "Value";
                sheet.Cells[2, 2].Value = "Test";
                sheet.Cells[3, 2].Value = "A";
                sheet.Cells[4, 2].Value = null;
                sheet.Cells[5, 2].Value = "4";

                package.SaveAs(target);

                target.Seek(0, SeekOrigin.Begin);

                resolver.Setup(pr => pr.EnumerateAvailablePlugins()).Returns(new[] { new Plugin() });
                uriResolver.Setup(ur => ur.ResolveToStream("file.xlsx", UriResolveMode.Read)).Returns(Task.FromResult<Stream>(target));

                var context = new ConnectQlContext(resolver.Object) { UriResolver = uriResolver.Object };
                var executeResult = await context.ExecuteAsync($"IMPORT PLUGIN 'Excel' {query}");

                Assert.Equal(expectedRecordCount, await executeResult.QueryResults[0].Rows.CountAsync());
            }
        }
    }
}
