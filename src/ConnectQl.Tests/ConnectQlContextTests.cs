using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectQl.Tests
{
    using System.Threading.Tasks;

    using ConnectQl.AsyncEnumerables;

    using JetBrains.Annotations;

    using Xunit;

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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory(DisplayName = "ExecuteAsync should return a result set with the specified number of items")]
        [InlineData("SELECT * FROM SPLIT('1,2,3', ',') splitted", 1, 3)]
        [InlineData("SELECT * FROM SPLIT('1,2,3', ',') splitted SELECT * FROM SPLIT('1,2,3', ',') splitted", 2, 3)]
        [InlineData("SELECT splitted.item FROM SPLIT('1,2,3', ',') splitted GROUP BY splitted.Item", 1, 3)]
        [InlineData("SELECT COUNT(splitted.item) FROM SPLIT('1,2,3', ',') splitted GROUP BY 0", 1, 1)]
        [InlineData("SELECT AVG(INT(splitted.item)) FROM SPLIT('1,2,3', ',') splitted GROUP BY 0", 1, 1)]
        public async Task CurrentShouldBeDefault([NotNull] string query, int numResults, int firstResultSetCount)
        {
            var context = new ConnectQlContext();
            var result = await context.ExecuteAsync(query);

            Assert.Equal(numResults, result.QueryResults.Count);
            Assert.Equal(firstResultSetCount, await result.QueryResults[0].Rows.CountAsync());
        }
    }
}
