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
    using System.Linq;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerables;

    using JetBrains.Annotations;

    using Xunit;

    /// <summary>
    /// Tests for the <see cref="Parser" /> class.
    /// </summary>
    [Trait("Category", "Parser")]
    public class ParserTests
    {
        /// <summary>
        /// Numbers should be parsed correctly.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="value">
        /// The exprected value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "Numbers should be parsed correctly. ")]
        [InlineData("SELECT 1.0", 1.0d)]
        [InlineData("SELECT 9223372036854775807", long.MaxValue)]
        [InlineData("SELECT -9223372036854775808", long.MinValue)]
        [InlineData("SELECT 2147483647", int.MaxValue)]
        [InlineData("SELECT -2147483648", int.MinValue)]
        [InlineData("SELECT STRING(1 WEEK + 2 DAY + 3 HOUR + 4 MINUTE + 5 SECOND + 6 MILLISECOND)", "9.03:04:05.0060000")]
        [InlineData("SELECT STRING(1 WEEKS + 2 DAYS + 3 HOURS + 4 MINUTES + 5 SECONDS + 6 MILLISECONDS)", "9.03:04:05.0060000")]
        public async Task NumbersShouldBeParsedCorrectly([NotNull] string query, object value)
        {
            var executeResult = await new ConnectQlContext().ExecuteAsync(query);

            var row = await executeResult.QueryResults[0].Rows.FirstAsync();

            Assert.Equal(value, row[row.ColumnNames[0]]);
        }

        /// <summary>
        /// Parser should throw on invalid number.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="message">
        /// The expected message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "Parser should throw on invalid number. ")]
        [InlineData("SELECT 9223372036854775808", "Invalid number (too long).")]
        [InlineData("SELECT -9223372036854775809", "Invalid number (too long).")]
        [InlineData("SELECT 1 INVALID_TIME_UNIT", "Invalid time unit specified: 'INVALID_TIME_UNIT', must be MILLISECOND[S], SECOND[S], MINUTE[S], HOUR[S], DAY[S] or WEEK[S].")]
        public async Task ParserShouldThrowOnInvalidNumber(string query, string message)
        {
            var exception = await Assert.ThrowsAsync<ExecutionException>(async () =>
            {
                await new ConnectQlContext().ExecuteAsync(query);
            });

            Assert.Equal(message, exception.Messages.First().Text);
        }
    }
}
