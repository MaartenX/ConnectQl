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
    }
}
