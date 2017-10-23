using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectQl.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ConnectQl.AsyncEnumerables;
    using Xunit;

    [Trait("Category", "Parser")]
    public class ParserTests
    {
        [Theory(DisplayName = "Numbers should be parsed correctly. ")]
        [InlineData("SELECT 1.0 FROM SPLIT('','') s", 1.0d)]
        [InlineData("SELECT 9223372036854775807 FROM SPLIT('','') s", long.MaxValue)]
        [InlineData("SELECT -9223372036854775808 FROM SPLIT('','') s", long.MinValue)]
        [InlineData("SELECT 2147483647 FROM SPLIT('','') s", int.MaxValue)]
        [InlineData("SELECT -2147483648 FROM SPLIT('','') s", int.MinValue)]
        [InlineData("SELECT STRING(1 WEEK + 2 DAY + 3 HOUR + 4 MINUTE + 5 SECOND + 6 MILLISECOND) FROM SPLIT('','') s", "9.03:04:05.0060000")]
        [InlineData("SELECT STRING(1 WEEKS + 2 DAYS + 3 HOURS + 4 MINUTES + 5 SECONDS + 6 MILLISECONDS) FROM SPLIT('','') s", "9.03:04:05.0060000")]
        public async Task NumbersShouldBeParsedCorrectly(string query, object value)
        {
            var executeResult = await new ConnectQlContext().ExecuteAsync(query);

            var row = await executeResult.QueryResults[0].Rows.FirstAsync();

            Assert.Equal(value, row[row.ColumnNames[0]]);
        }

        [Theory(DisplayName = "Parser should throw on invalid number. ")]
        [InlineData("SELECT 9223372036854775808 FROM SPLIT('','') s", "Invalid number (too long).")]
        [InlineData("SELECT -9223372036854775809 FROM SPLIT('','') s", "Invalid number (too long).")]
        [InlineData("SELECT 1 INVALID_TIME_UNIT FROM SPLIT('','') s", "Invalid time unit specified: 'INVALID_TIME_UNIT', must be MILLISECOND[S], SECOND[S], MINUTE[S], HOUR[S], DAY[S] or WEEK[S].")]
        public async Task ParserShouldThrow(string query, string message)
        {
            var exception = await Assert.ThrowsAsync<ExecutionException>(async () =>
            {
                await new ConnectQlContext().ExecuteAsync(query);
            });

            Assert.Equal(message, exception.Messages.First().Text);
        }
    }
}
