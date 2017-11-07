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

namespace ConnectQl.Plugins
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using ConnectQl.AsyncEnumerables;
    using ConnectQl.DataSources;
    using ConnectQl.FileFormats;
    using ConnectQl.Interfaces;
    using ConnectQl.Results;
    using ConnectQl.Triggers;

    using JetBrains.Annotations;

    /// <summary>
    /// The default functions.
    /// </summary>
    internal class DefaultFunctions : IConnectQlPlugin
    {
        /// <summary>
        /// The date diff type.
        /// </summary>
        private enum DateDiffType
        {
            /// <summary>
            /// The millisecond.
            /// </summary>
            Millisecond = 0,

            /// <summary>
            /// The milliseconds.
            /// </summary>
            Milliseconds = 0,

            /// <summary>
            /// The second.
            /// </summary>
            Second = 1,

            /// <summary>
            /// The seconds.
            /// </summary>
            Seconds = 1,

            /// <summary>
            /// The minute.
            /// </summary>
            Minute = 2,

            /// <summary>
            /// The minutes.
            /// </summary>
            Minutes = 2,

            /// <summary>
            /// The hour.
            /// </summary>
            Hour = 3,

            /// <summary>
            /// The hours.
            /// </summary>
            Hours = 3,

            /// <summary>
            /// The day.
            /// </summary>
            Day = 4,

            /// <summary>
            /// The days.
            /// </summary>
            Days = 4,

            /// <summary>
            /// The week.
            /// </summary>
            Week = 5,

            /// <summary>
            /// The weeks.
            /// </summary>
            Weeks = 5,

            /// <summary>
            /// The month.
            /// </summary>
            Month = 6,

            /// <summary>
            /// The months.
            /// </summary>
            Months = 6,

            /// <summary>
            /// The year.
            /// </summary>
            Year = 7,

            /// <summary>
            /// The years.
            /// </summary>
            Years = 7,
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        [NotNull]
        public string Name => "DefaultFunctions";

        /// <summary>
        /// The register plugin.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void RegisterPlugin([NotNull] IPluginContext context)
        {
            context.FileFormats.Add(new CsvFileFormat());

            context.Functions
                .AddWithoutSideEffects("datediff", (DateDiffType type, DateTime first, DateTime second) => DefaultFunctions.DateDiff(type, first, second))
                .SetDescription("Calculates the difference between two date/times.", "The type of unit to use when calculating the difference.", "The first date/time.", "The second date/time.")
                .AddWithoutSideEffects("dateadd", (DateDiffType type, DateTime datetime, double value) => DefaultFunctions.DateAdd(type, datetime, value))
                .SetDescription("Adds an offset to the date/time.", "The type of unit to use when adding the offset.", "The date/time.", "The number of units to add.")
                .AddWithoutSideEffects("utcnow", () => DateTime.UtcNow)
                .SetDescription("Gets the current UTC time.")
                .AddWithoutSideEffects("todate", (int year, int month, int day) => new DateTime(year, month, day))
                .SetDescription("Converts a year, month and day to a date/time.", "The years.", "The months.", "The days.")
                .AddWithoutSideEffects("date", (string value) => DefaultFunctions.ParseDate(value))
                .SetDescription("Parses the date.", "The string to parse into a date")
                .AddWithoutSideEffects("date", (string value, string pattern) => DefaultFunctions.ParseDate(value, pattern))
                .SetDescription("Parses the date.", "The string to parse into a date", "The format to use")
                .AddWithoutSideEffects("now", () => DateTime.Now)
                .SetDescription("Gets the current local time.")
                .AddWithoutSideEffects("today", () => DateTime.Today)
                .SetDescription("Gets the current local time at last midnight.")
                .AddWithoutSideEffects("upper", (string value) => value.ToUpperInvariant())
                .SetDescription("Converts a string to uppercase.", "The string to convert.")
                .AddWithoutSideEffects("lower", (string value) => value.ToLowerInvariant())
                .SetDescription("Converts a string to lowercase.", "The string to convert.")
                .AddWithoutSideEffects("int", (object value) => DefaultFunctions.ToInt(value))
                .SetDescription("Converts a value to an int.", "The value to convert.")
                .AddWithoutSideEffects("float", (object value) => DefaultFunctions.ToFloat(value))
                .SetDescription("Converts a value to a float.", "The value to convert.")
                .AddWithoutSideEffects("floor", (double? value) => value == null ? null : (double?)Math.Floor(value.Value))
                .SetDescription("Returns the largest integer less than or equal to the value.", "The value.")
                .AddWithoutSideEffects("string", (object value) => (value ?? string.Empty).ToString())
                .SetDescription("Converts a value to a string.", "The value to convert.")
                .AddWithoutSideEffects("object", (object value) => value)
                .SetDescription("Converts a value to an object.", "The value to convert.")
                .AddWithoutSideEffects("datetime", (string value) => DefaultFunctions.Parse(value))
                .SetDescription("Converts a string to a date/time.", "The value to convert.")
                .AddWithoutSideEffects("datetime", (int year, int month, int day) => new DateTime(year, month, day))
                .SetDescription("Converts a year, month and day to a date/time.", "The years.", "The months.", "The days.")
                .AddWithoutSideEffects("datetime", (int year, int month, int day, int hour, int minute, int second) => new DateTime(year, month, day, hour, minute, second))
                .SetDescription("Converts a year, month, day, hour, minute and second to a date/time.", "The years.", "The months.", "The days.", "The hours.", "The minutes.", "The seconds.")
                .AddWithoutSideEffects("isnull", (object first, object second) => first ?? second)
                .SetDescription("Returns the first value if it is not null, otherwise returns the second value.", "The first value.", "The second value.")
                .AddWithoutSideEffects("if", (bool condition, object isTrue, object isFalse) => condition ? isTrue : isFalse)
                .SetDescription("Checks the condition, and if it's true, returns the first value, otherwise the second value.", "The condition.", "The first value.", "The second value.")
                .AddWithoutSideEffects("year", (DateTime value) => value.Year)
                .SetDescription("Gets the year part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("month", (DateTime value) => value.Month)
                .SetDescription("Gets the month part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("day", (DateTime value) => value.Day)
                .SetDescription("Gets the day part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("hour", (DateTime value) => value.Hour)
                .SetDescription("Gets the hour part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("minute", (DateTime value) => value.Minute)
                .SetDescription("Gets the minute part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("second", (DateTime value) => value.Second)
                .SetDescription("Gets the second part of a date/time.", "The date/time.")
                .AddWithoutSideEffects("weekday", (DateTime value) => value.DayOfWeek)
                .SetDescription("Gets the day number in the week of a date/time.", "The date/time.")
                .AddWithoutSideEffects("weekend", (DateTime value) => value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday)
                .SetDescription("Returns true if the date/time is in the weekend, false otherwise.", "The date/time.")
                .AddWithoutSideEffects("weeknum", (DateTime value) => DefaultFunctions.GetIso8601WeekOfYear(value))
                .SetDescription("Returns the ISO 8601 week number of the date/time.", "The date/time.")
                .AddWithoutSideEffects("localtime", (DateTime value) => (value.Kind != DateTimeKind.Local ? value.ToLocalTime() : value).ToString("o"))
                .SetDescription("Converts the date/time to local time if the date/time is universal.", "The date/time.")
                .AddWithoutSideEffects("utc", (DateTime value) => (value.Kind != DateTimeKind.Utc ? value.ToUniversalTime() : value).ToString("o"))
                .SetDescription("Converts the date/time to UTC time if the date/time is local.", "The date/time.")
                .AddWithoutSideEffects("tickstamp", (DateTime value) => DateTime.MaxValue.Ticks - value.ToUniversalTime().Ticks)
                .SetDescription("Gets the tick stamp (maximum ticks - utc time ticks) of the date/time.", "The date/time.")
                .AddWithoutSideEffects("inversetimestamp", (DateTime value) => DefaultFunctions.ToInverseDateStamp(value))
                .SetDescription("Gets the inverse time stamp (100000000000000 - YYYYMMddHHmmss) of the date/time.", "The date/time.")
                .AddWithoutSideEffects("timestamp", (DateTime value) => DefaultFunctions.ToDateStamp(value))
                .SetDescription("Gets the time stamp (YYYYMMddHHmmss) of the date/time.", "The date/time.")
                .AddWithoutSideEffects("unixtimestamp", (DateTime value) => DefaultFunctions.ToUnixTimestamp(value))
                .SetDescription("Gets the unix time stamp of the date/time.", "The date/time.")
                .AddWithoutSideEffects("avg", (IAsyncEnumerable<double?> value) => value.AverageAsync())
                .SetDescription("Gets the average of the values in a group.", "The value.")
                .AddWithoutSideEffects("sum", (IAsyncEnumerable<double?> value) => value.SumAsync())
                .SetDescription("Gets the sum of the values in a group.", "The value.")
                .AddWithoutSideEffects("min", (IAsyncEnumerable<object> value) => value.MinAsync())
                .SetDescription("Gets the minimum of the values in a group.", "The value.")
                .AddWithoutSideEffects("min", (double? first, double? second) => first != null && second != null ? Math.Min(first.Value, second.Value) : first ?? second)
                .SetDescription("Gets the minimum of two values.", "The first value.", "The second value.")
                .AddWithoutSideEffects("max", (IAsyncEnumerable<object> value) => value.MaxAsync())
                .SetDescription("Gets the maximum of the values in a group.", "The value.")
                .AddWithoutSideEffects("max", (double? first, double? second) => first != null && second != null ? Math.Max(first.Value, second.Value) : first ?? second)
                .SetDescription("Gets the maximum of two values.", "The first value.", "The second value.")
                .AddWithoutSideEffects("first", (IAsyncEnumerable<object> value) => value.FirstAsync())
                .SetDescription("Gets the first of the values in a group.", "The value.")
                .AddWithoutSideEffects("last", (IAsyncEnumerable<object> value) => value.LastAsync())
                .SetDescription("Gets the last of the values in a group.", "The value.")
                .AddWithoutSideEffects("count", (IAsyncEnumerable<object> value) => value.CountAsync())
                .SetDescription("Gets the count of the values in a group.", "The value.")
                .AddWithoutSideEffects("countdistinct", (IAsyncEnumerable<object> value) => value.Distinct().CountAsync())
                .SetDescription("Gets the count of the distinct values in a group.", "The value.")
                .AddWithoutSideEffects("join", (IAsyncEnumerable<string> value, string separator) => value.AggregateAsync(new StringBuilder(), (builder, item) => builder.Append(separator).Append(item)).ToString())
                .SetDescription("Joins the values in a group using a separator.", "The value.", "The separator.")
                .AddWithoutSideEffects("groupconcat", (IAsyncEnumerable<string> value) => value.AggregateAsync(new StringBuilder(), (sb, s) => sb.Append(s)).ToString())
                .SetDescription("Concatenates the values in a group using a separator.", "The value.")
                .AddWithoutSideEffects("time", (TimeSpan past, TimeSpan future) => new TimeDataSource(TimeDataSource.TimeOffset.Midnight, past, future, TimeSpan.FromMinutes(15)))
                .SetDescription("Creates a data source that returns rows containing a column 'Time' for every moment between past and future in intervals of 15 minutes.", "The amount of time in the past.", "The amount of time in the future.")
                .AddWithoutSideEffects("time", (TimeSpan past, TimeSpan future, TimeSpan interval) => new TimeDataSource(TimeDataSource.TimeOffset.Midnight, past, future, interval))
                .SetDescription("Creates a data source that returns rows containing a column 'Time' for every moment between past and future in the specified interval.", "The amount of time in the past.", "The amount of time in the future.", "The interval")
                .AddWithoutSideEffects("time", (TimeDataSource.TimeOffset moment, TimeSpan past, TimeSpan future, TimeSpan interval) => new TimeDataSource(moment, past, future, interval))
                .SetDescription("Creates a data source that returns rows containing a column 'Time' for every moment between past and future in the specified interval.", "The moment to calculate the times around.", "The amount of time in the past.", "The amount of time in the future.", "The interval")
                .AddWithoutSideEffects("file", (string uri) => new FileDataSource(uri))
                .SetDescription("Creates a connection to a file.", "The name of the file.")
                .AddWithoutSideEffects("file", (string uri, string encoding) => new FileDataSource(uri, Encoding.GetEncoding(encoding)))
                .SetDescription("Creates a connection to a file.", "The name of the file.", "The encoding of the file.")
                .AddWithoutSideEffects("split", (string value, string separator) => EnumerableDataSource.Create(value.Split(new[] { separator }, StringSplitOptions.None)))
                .SetDescription("Creates a data source containing a column 'Item' containing the splitted value.", "The value to split.", "The separator to use.")
                .AddWithoutSideEffects("regexreplace", (string value, string regex, string replacement) => Regex.Replace(value, regex, replacement, RegexOptions.ECMAScript))
                .SetDescription("Replaces all matches of regex in value with replacement and returns the result.", "The value.", "The regular expression to find.", "The text to replace a match with.")
                .AddWithoutSideEffects("trim", (string value) => value.Trim())
                .SetDescription("Trims whitespace of a value.", "The value.")
                .AddWithoutSideEffects("trimstart", (string value) => value.TrimStart())
                .SetDescription("Trims leading whitespace of a value.", "The value.")
                .AddWithoutSideEffects("trimend", (string value) => value.TrimEnd())
                .SetDescription("Trims trailing whitespace of a value.", "The value.")
                .AddWithoutSideEffects("triggerafter", (string jobName) => DefaultFunctions.AfterJob(jobName))
                .SetDescription("Triggers a job after the specified job.", "The name of the specified job.")
                .AddWithoutSideEffects("triggerevery", (TimeSpan interval) => DefaultFunctions.Interval(interval))
                .SetDescription("Triggers a job after every interval.", "The interval.")
                .AddWithoutSideEffects("temp", () => new TempDataSource())
                .SetDescription("Creates a temporary data source.")
                .AddWithoutSideEffects("dcast", (IAsyncEnumerable<Row> values, string columnName, string columnValue) => new DCast(values, columnName, columnValue, DCastFunction.First))
                .SetDescription("Creates a data source with value as the column name, and value as true.", "The rows", "The value to create column names from.", "The values for the column.")
                .AddWithoutSideEffects("dcast", (IAsyncEnumerable<Row> values, string columnName, string columnValue, DCastFunction function) => new DCast(values, columnName, columnValue, function))
                .SetDescription("Creates a data source with value as the column name, and value as true.", "The rows", "The value to create column names from.", "The values for the column.", "The function to use when casting multiple values.")
                .AddWithoutSideEffects("tocolumn", (string columnName) => ColumnDataSource.Create(columnName))
                .SetDescription("Creates a data source with value as the column name, and value as true.", "The value to create column names from.")
                .AddWithoutSideEffects("tocolumn", (string columnName, object columnValue) => ColumnDataSource.Create(columnName, columnValue))
                .SetDescription("Creates a data source with value as the column name, and value as true.", "The value to create column names from.", "The value to create column values from.")
                .AddWithoutSideEffects("classify", (string value, string classes, string values) => DefaultFunctions.Classify(value, classes, values, ",", null))
                .SetDescription("Looks up the value in the comma separated classes, an replaces it with the value at the same index in the comma separatored values.", "The value to look up.", "The classes, separated by commas.", "The values, separated by commas.")
                .AddWithoutSideEffects("classify", (string value, string separator, string classes, string values) => DefaultFunctions.Classify(value, classes, values, separator, null))
                .SetDescription("Looks up the value in the classes separated by separator, an replaces it with the value at the same index in the values separated by separator.", "The value to look up.", "The separator", "The classes, separated by commas.", "The values, separated by commas.")
                .AddWithoutSideEffects("classify", (string value, string separator, string classes, string values, string defaultValue) => DefaultFunctions.Classify(value, classes, values, separator, defaultValue))
                .SetDescription("Looks up the value in the classes separated by separator, an replaces it with the value at the same index in the values separated by separator. When value is not found, defaultValue is returned.", "The value to look up.", "The separator", "The classes, separated by commas.", "The values, separated by commas.", "The default value.");
        }

        /// <summary>
        /// Converts a value to an int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The value as int or <c>null</c> if it cannot be converted.
        /// </returns>
        private static int? ToInt([CanBeNull] object value)
        {
            return int.TryParse(value as string ?? value?.ToString() ?? string.Empty, out var result) ? (int?)result : null;
        }

        /// <summary>
        /// Converts a value to a float.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The value as float or <c>null</c> if it cannot be converted.
        /// </returns>
        private static float? ToFloat([CanBeNull] object value)
        {
            return float.TryParse(value as string ?? value?.ToString() ?? string.Empty, out var result) ? (int?)result : null;
        }

        /// <summary>
        /// The interval trigger.
        /// </summary>
        /// <param name="jobName">
        /// The job name.
        /// </param>
        /// <returns>
        /// The <see cref="ITrigger"/>.
        /// </returns>
        [NotNull]
        private static ITrigger AfterJob(string jobName)
        {
            return new AfterJobTrigger(jobName);
        }

        /// <summary>
        /// Classifies the value by lookup it up in the <paramref name="classes"/> and returning the corresponding
        ///     <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The value to classify.
        /// </param>
        /// <param name="classes">
        /// The options to find, separated by <paramref name="separator"/>.
        /// </param>
        /// <param name="values">
        /// The values to replace the classes with, separated by <paramref name="separator"/>.
        /// </param>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The value belonging to the classes, or defaultValue if it's not null, or value if defaultValue is null.
        /// </returns>
        private static string Classify(string value, [NotNull] string classes, [NotNull] string values, string separator, [CanBeNull] string defaultValue)
        {
            var splittedValues = values.Split(new[] { separator, }, StringSplitOptions.None);
            var splittedClasses = classes.Split(new[] { separator, }, StringSplitOptions.None);

            var index = Array.IndexOf(splittedClasses, value);

            return index == -1 || index >= splittedValues.Length ? defaultValue ?? value : splittedValues[index];
        }

        /// <summary>
        /// Adds a value to the specified date.
        /// </summary>
        /// <param name="type">
        /// The type of date difference that will be added.
        /// </param>
        /// <param name="datetime">
        /// The date/time.
        /// </param>
        /// <param name="value">
        /// The value to add.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an invalid date difference type is provided.
        /// </exception>
        private static DateTime DateAdd(DateDiffType type, DateTime datetime, double value)
        {
            switch (type)
            {
                case DateDiffType.Millisecond:
                    return datetime.AddMilliseconds(value);
                case DateDiffType.Second:
                    return datetime.AddSeconds(value);
                case DateDiffType.Minute:
                    return datetime.AddMinutes(value);
                case DateDiffType.Hour:
                    return datetime.AddHours(value);
                case DateDiffType.Day:
                    return datetime.AddDays(value);
                case DateDiffType.Week:
                    return datetime.AddDays(value * 7);
                case DateDiffType.Month:
                    return datetime.AddMonths((int)value);
                case DateDiffType.Year:
                    return datetime.AddYears((int)value);
                default:
                    throw new Exception("Invalid date diff type.");
            }
        }

        /// <summary>
        /// Calculates the difference between two dates.
        /// </summary>
        /// <param name="type">
        /// The type of the difference to calculate.
        /// </param>
        /// <param name="first">
        /// The first date.
        /// </param>
        /// <param name="second">
        /// The second date.
        /// </param>
        /// <returns>
        /// The difference.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an invalid date difference type is provided.
        /// </exception>
        private static int DateDiff(DateDiffType type, DateTime first, DateTime second)
        {
            if (second > first)
            {
                var tmp = second;
                second = first;
                first = tmp;
            }

            switch (type)
            {
                case DateDiffType.Millisecond:
                    return (int)(second - first).TotalMilliseconds;
                case DateDiffType.Second:
                    return (int)(second - first).TotalSeconds;
                case DateDiffType.Minute:
                    return (int)(second - first).TotalMinutes;
                case DateDiffType.Hour:
                    return (int)(second - first).TotalHours;
                case DateDiffType.Day:
                    return (int)(second - first).TotalDays;
                case DateDiffType.Week:
                    return (int)(second - first).TotalDays / 7;
                case DateDiffType.Month:
                    return (second.Year - first.Year) * 12 + second.Month - first.Month + (second.Day >= first.Day ? 0 : -1);
                case DateDiffType.Year:
                    var years = second.Year - first.Year;
                    return years - (first > second.AddYears(-years) ? 1 : 0);
                default:
                    throw new Exception("Invalid date diff type.");
            }
        }

        /// <summary>
        /// This presumes that weeks start with Monday.
        ///     Week 1 is the 1st week of the year with a Thursday in it.
        /// </summary>
        /// <param name="time">
        /// The time to get the week of the year for.
        /// </param>
        /// <returns>
        /// The week number.
        /// </returns>
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// The interval trigger.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <returns>
        /// The <see cref="ITrigger"/>.
        /// </returns>
        [NotNull]
        private static ITrigger Interval(TimeSpan interval)
        {
            return new IntervalTrigger(interval);
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime Parse(string value)
        {
            return DateTime.TryParse(value, out var result) ? result : DateTime.MinValue;
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to a date stamp <c>(100000000000000 - yyyyMMddHHmmss)</c>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private static long ToDateStamp(DateTime value)
        {
            return value.Year * 10000000000 + value.Month * 100000000 + value.Day * 1000000 + value.Hour * 10000 + value.Minute * 100 + value.Second;
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to a date stamp <c>(100000000000000 - yyyyMMddHHmmss)</c>.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private static long ToInverseDateStamp(DateTime value)
        {
            return 100000000000000 - DefaultFunctions.ToDateStamp(value);
        }

        /// <summary>
        /// Converts the <see cref="DateTime"/> to a unix timestamp.
        /// </summary>
        /// <param name="date">
        /// The date to convert.
        /// </param>
        /// <returns>
        /// The unix timestamp.
        /// </returns>
        private static long ToUnixTimestamp(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(date.ToUniversalTime() - epoch).TotalSeconds;
        }

        /// <summary>
        /// Parses the date.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// The parsed date or <c>null</c>.
        /// </returns>
        private static DateTime? ParseDate(string value)
        {
            return DateTime.TryParse(value, out var result) ? (DateTime?)result : null;
        }

        /// <summary>
        /// Parses the date.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="pattern">The pattern to use.</param>
        /// <returns>
        /// The parsed date or <c>null</c>.
        /// </returns>
        private static DateTime? ParseDate(string value, string pattern)
        {
            return DateTime.TryParseExact(value, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                       ? (DateTime?)result
                       : null;
        }
    }
}