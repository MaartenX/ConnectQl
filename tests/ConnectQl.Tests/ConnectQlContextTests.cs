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

namespace ConnectQl.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.Intellisense;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;

    using JetBrains.Annotations;

    using Moq;

    using Xunit;

    /// <summary>
    /// Tests for the <see cref="ConnectQlContext"/>.
    /// </summary>
    [Trait("Category", "ConnectQlContext")]
    public class ConnectQlContextTests
    {
        /// <summary>
        /// ExecuteAsync should return a result set with the specified number of items.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <param name="numResults">
        /// The number of results that should be returned.
        /// </param>
        /// <param name="firstResultSetCount">
        /// The number of results in the first result.
        /// </param>
        /// <param name="firstResultValue">
        /// The first value in the result.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "ExecuteAsync should return a result set with the specified number of items. ")]
        [InlineData("SELECT * FROM SPLIT('1,2,3', ',') splitted", 1, 3, "1")]
        [InlineData("SELECT * FROM SPLIT('1,2,3', ',') splitted ORDER BY splitted.item DESC", 1, 3, "3")]
        [InlineData("SELECT * FROM SPLIT('1,2,3', ',') splitted SELECT * FROM SPLIT('1,2,3', ',') splitted", 2, 3, "1")]
        [InlineData("SELECT splitted.item FROM SPLIT('1,2,3', ',') splitted GROUP BY splitted.Item", 1, 3, "1")]
        [InlineData("SELECT COUNT(splitted.item) FROM SPLIT('1,2,3', ',') splitted GROUP BY 0", 1, 1, 3L)]
        [InlineData("SELECT AVG(INT(splitted.item)) FROM SPLIT('1,2,3', ',') splitted GROUP BY 0", 1, 1, 2d)]
        [InlineData("DECLARE @a = SPLIT('1,2,3', ',') SELECT AVG(INT(splitted.item)) FROM @a splitted GROUP BY 0", 1, 1, 2d)]
        public async Task ExecuteAsyncShouldReturnResult([NotNull] string query, int numResults, int firstResultSetCount, object firstResultValue)
        {
            var context = new ConnectQlContext();
            var result = await context.ExecuteAsync(query);

            Assert.Equal(numResults, result.QueryResults.Count);
            Assert.Equal(firstResultSetCount, await result.QueryResults[0].Rows.CountAsync());

            var row = await result.QueryResults[0].Rows.FirstAsync();

            Assert.Equal(firstResultValue, row[row.ColumnNames[0]]);
        }

        /// <summary>
        /// ExecuteAsync should return a joined set.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <param name="resultValues">
        /// The results to compare with.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "ExecuteAsync should return a joined set. ")]
        [InlineData("SELECT a.Item FROM SPLIT('31,22,11', ',') a INNER JOIN SPLIT('22,11,11', ',') b ON INT(a.Item)=INT(b.Item) ORDER BY a.Item ASC", new[] { "11", "11", "22" })]
        [InlineData("SELECT a.Item FROM SPLIT('31,22,11', ',') a LEFT JOIN SPLIT('22,11,11', ',') b ON INT(a.Item)=INT(b.Item) ORDER BY a.Item ASC", new[] { "11", "11", "22", "31" })]
        [InlineData("SELECT a.Item FROM SPLIT('31,22,11', ',') a LEFT JOIN SPLIT('22,44', ',') b ON INT(a.Item)=INT(b.Item) ORDER BY a.Item ASC", new[] { "11", "22", "31" })]
        [InlineData("SELECT INT(a.Item) AS Item FROM SPLIT('31,22,11', ',') a LEFT JOIN SPLIT('22,44', ',') b ON INT(a.Item)=INT(b.Item) ORDER BY a.Item ASC", new object[] { 11, 22, 31 })]
        public async Task ExecuteAsyncShouldReturnJoinedSet([NotNull] string query, object[] resultValues)
        {
            var context = new ConnectQlContext();
            var result = await context.ExecuteAsync(query);

            Assert.Equal(1, result.QueryResults.Count);

            var array = await result.QueryResults[0].Rows.Select(r => r["Item"]).ToArrayAsync();

            Assert.Equal(resultValues, array);
        }

        /// <summary>
        /// The intellisense should return correct tokens.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "Intellisense should return the correct number of tokens. ")]
        [InlineData("IMPORT PLUGIN 'Dummy' INSERT INTO dummy() SELECT 'a' + 'b'", 15)]
        [InlineData("SELECT * FROM SPLIT('1', '2') s", 13)]
        [InlineData("-- SELECT * FROM SPLIT('1', '2') s", 3)]
        public async Task IntellisenseShouldReturnCorrectTokens(string query, int tokens)
        {
            using (var context = new ConnectQlContext().CreateIntellisenseSession())
            {
                var tcs = new TaskCompletionSource<IDocumentDescriptor>();

                new CancellationTokenSource(2000).Token.Register(() => tcs.TrySetCanceled(), false);

                context.DocumentUpdated += (sender, args) =>
                                           {
                                               if (args.Document.Tokens != null)
                                               {
                                                   tcs.SetResult(args.Document);
                                               }
                                           };

                context.UpdateDocument("file1.connectql", query, 1);

                var doc = await tcs.Task;

                Assert.Equal(tokens, doc.Tokens.Count);
            }
        }

        /// <summary>
        /// Disposed context should throw on all public methods.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact(DisplayName = "Disposed context should throw on all public methods.")]
        public async Task DisposedContextShouldThrow()
        {
            var context = new ConnectQlContext();

            ((IDisposable)context).Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => context.ExecuteAsync("SELECT NULL"));

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("SELECT NULL")))
            {
                await Assert.ThrowsAsync<ObjectDisposedException>(() => context.ExecuteAsync("file.connectql", stream));
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("SELECT NULL")))
            {
                await Assert.ThrowsAsync<ObjectDisposedException>(() => context.ExecuteAsync(stream));
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(() => context.ExecuteFileAsync("filename"));
        }

        /// <summary>
        /// Operators should return the correct type.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "Operators should return the correct type. ")]
        [InlineData("SELECT 'a' + 'b'", "ab")]
        [InlineData("SELECT 'a' + OBJECT('b')", "ab")]
        [InlineData("SELECT OBJECT('a') + 'b'", "ab")]
        [InlineData("SELECT OBJECT('a') + OBJECT('b')", "ab")]
        [InlineData("SELECT OBJECT('a') + OBJECT(1)", "a1")]
        [InlineData("SELECT OBJECT(1) + OBJECT('a')", "1a")]
        [InlineData("SELECT OBJECT(1) * OBJECT(1.0) + 'ábc' SELECT OBJECT(1) * OBJECT(1.0) + 'ábc' SELECT OBJECT(1) + OBJECT(1.0) + 'ábc' SELECT OBJECT(1) * OBJECT(1.0) + 'ábc' SELECT OBJECT(1) + OBJECT(1.0) + 'ábc' FROM SPLIT('1,1,1,1,1,1,1,1,1,1,1,1,1,1,1', ',') s", "1ábc")]
        [InlineData("SELECT 111 > NULL", true)]
        [InlineData("SELECT INT('12') > INT(111)", false)]
        public async Task OperatorsShouldReturnCorrectType(string query, object value)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl");

            using (var context = new ConnectQlContext())
            {
                var executeResult = await context.ExecuteAsync(query);

                var row = await executeResult.QueryResults[0].Rows.FirstAsync();

                Assert.Equal(value, row[row.ColumnNames[0]]);
            }
        }
    }
}
