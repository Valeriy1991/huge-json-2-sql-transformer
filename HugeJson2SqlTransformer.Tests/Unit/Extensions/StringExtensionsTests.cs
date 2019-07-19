using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Extensions;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Extensions
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class StringExtensionsTests
    {
        [Fact]
        public void FixSingleQuotes_InputStringIsNullOrEmpty_ReturnNull()
        {
            // Arrange
            string inputString = null;
            // Act
            var fixedString = inputString.FixSingleQuotes();
            // Assert
            Assert.Null(fixedString);
        }

        [Theory]
        [InlineData("abcd")]
        [InlineData("")]
        [InlineData("  ")]
        public void FixSingleQuotes_InputStringDoesNotContainsSingleQuotes_ReturnUnchangedString(string inputString)
        {
            // Arrange
            // Act
            var fixedString = inputString.FixSingleQuotes();
            // Assert
            Assert.Equal(inputString, fixedString);
        }

        [Fact]
        public void FixSingleQuotes_InputStringContainsSingleQuotes_ReturnStringWithDoubleSingleQuotes()
        {
            // Arrange
            var inputString = "abcde'";
            var correctFixedString = inputString.Replace("'", "''");
            // Act
            var fixedString = inputString.FixSingleQuotes();
            // Assert
            Assert.Equal(correctFixedString, fixedString);
        }
    }
}