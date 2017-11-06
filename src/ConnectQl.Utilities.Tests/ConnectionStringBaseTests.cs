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

namespace ConnectQl.Utilities.Tests
{
    using System;
    using Xunit;

    /// <summary>
    /// Test for <see cref="ConnectionStringBase{T}"/>.
    /// </summary>
    [Trait("Category", "ConnectionStringBase<T>")]
    public class ConnectionStringBaseTests
    {
        /// <summary>
        /// <see cref="ConnectionStringBase{T}" /> should parse all valid properties.
        /// </summary>
        [Fact(DisplayName = "ConnectionStringBase<T> should parse all valid properties.")]
        public void ShouldParseAllValidFields()
        {
            var parsed = ValidConnectionString.Parse("First=Test;Second=2;Third=true;Fourth=http://test.nl;Fifth=00000000-0000-0000-0000-0000000000000;");

            Assert.Equal("Test", parsed.First);
            Assert.Equal(2, parsed.Second);
            Assert.True(parsed.Third);
            Assert.Equal(new Uri("http://test.nl"), parsed.Fourth);
            Assert.Equal(Guid.Empty, parsed.Fifth);
        }

        /// <summary>
        /// <see cref="ConnectionStringBase{T}" /> should fill missing properties with defaults.
        /// </summary>
        [Fact(DisplayName = "ConnectionStringBase<T> should fill missing properties with defaults.")]
        public void ShouldFillMissingFieldsWithDefaults()
        {
            var parsed = ValidConnectionString.Parse(string.Empty);

            Assert.Equal(default(string), parsed.First);
            Assert.Equal(default(int), parsed.Second);
            Assert.Equal(default(bool), parsed.Third);
            Assert.Equal(default(Uri), parsed.Fourth);
            Assert.Equal(default(Guid), parsed.Fifth);
        }

        /// <summary>
        /// <see cref="ConnectionStringBase{T}" /> should fill invalid properties with defaults.
        /// </summary>
        [Fact(DisplayName = "ConnectionStringBase<T> should fill invalid properties with defaults.")]
        public void ShouldFillInvalidFieldsWithDefaults()
        {
            var parsed = ValidConnectionString.Parse(string.Empty);

            Assert.Equal(default(string), parsed.First);
            Assert.Equal(default(int), parsed.Second);
            Assert.Equal(default(bool), parsed.Third);
            Assert.Equal(default(Uri), parsed.Fourth);
            Assert.Equal(default(Guid), parsed.Fifth);
        }

        /// <summary>
        /// <see cref="ConnectionStringBase{T}" /> should support quotes and escaping.
        /// </summary>
        [Fact(DisplayName = "ConnectionStringBase<T> should support quotes and escaping.")]
        public void ShouldSupportQuotesAndEscaping()
        {
            var parsed = ValidConnectionString.Parse("First=\"'first\"\"\"");

            Assert.Equal("'first\"", parsed.First);
        }

        /// <summary>
        /// <see cref="ConnectionStringBase{T}" /> should fill unmarked properties.
        /// </summary>
        [Fact(DisplayName = "ConnectionStringBase<T> should not fill unmarked properties.")]
        public void ShouldNotFillUnmarkedProperties()
        {
            var parsed = ConnectionStringWithoutFields.Parse("First=Test;Second=2;Third=true;Fourth=http://test.nl;Fifth=00000000-0000-0000-0000-0000000000000;");

            Assert.Equal(default(string), parsed.First);
            Assert.Equal(default(int), parsed.Second);
            Assert.Equal(default(bool), parsed.Third);
            Assert.Equal(default(Uri), parsed.Fourth);
            Assert.Equal(default(Guid), parsed.Fifth);
        }

        /// <summary>
        /// A connection string without any properties marked as <see cref="ConnectionStringPartAttribute"/>.
        /// </summary>
        public class ConnectionStringWithoutFields : ConnectionStringBase<ConnectionStringWithoutFields>
        {
            /// <summary>
            /// Gets or sets the first property.
            /// </summary>
            public string First { get; set; }

            /// <summary>
            /// Gets or sets the second property.
            /// </summary>
            public int Second { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the third property is set.
            /// </summary>
            public bool Third { get; set; }

            /// <summary>
            /// Gets or sets the fourth property.
            /// </summary>
            public Uri Fourth { get; set; }

            /// <summary>
            /// Gets or sets the fifth property.
            /// </summary>
            public Guid Fifth { get; set; }
        }

        /// <summary>
        /// A valid connection string.
        /// </summary>
        public class ValidConnectionString : ConnectionStringBase<ValidConnectionString>
        {
            /// <summary>
            /// Gets or sets the first property.
            /// </summary>
            [ConnectionStringPart]
            public string First { get; set; }

            /// <summary>
            /// Gets or sets the second property.
            /// </summary>
            [ConnectionStringPart]
            public int Second { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the third property is set.
            /// </summary>
            [ConnectionStringPart]
            public bool Third { get; set; }

            /// <summary>
            /// Gets or sets the fourth property.
            /// </summary>
            [ConnectionStringPart]
            public Uri Fourth { get; set; }

            /// <summary>
            /// Gets or sets the fifth property.
            /// </summary>
            [ConnectionStringPart]
            public Guid Fifth { get; set; }
        }
    }
}
