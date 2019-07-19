using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Extensions;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Extensions
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class ListExtensionsTests
    {
        private readonly Faker _faker = new Faker();

        [Fact]
        public void AsJsonString_ListIsNull_ReturnEmptyString()
        {
            // Arrange
            List<string> list = null;
            // Act
            var jsonString = list.AsJsonString();
            // Assert
            Assert.Empty(jsonString);
        }

        [Fact]
        public void AsJsonString_ListIsEmpty_ReturnCorrectJsonString()
        {
            // Arrange
            var list = new List<string>();
            var correctJsonString = "[\n\n]";
            // Act
            var jsonString = list.AsJsonString();
            // Assert
            Assert.Equal(correctJsonString, jsonString);
        }

        [Fact]
        public void AsJsonString_ListIsNotEmpty_ReturnCorrectJsonString()
        {
            // Arrange
            var listItem1 = _faker.Person.FirstName;
            var listItem2 = _faker.Person.FirstName;
            var listItem3 = _faker.Person.FirstName;
            var list = new List<string>()
            {
                listItem1, listItem2, listItem3
            };
            var correctJsonString = $"[\n{listItem1},\n{listItem2},\n{listItem3}\n]";
            // Act
            var jsonString = list.AsJsonString();
            // Assert
            Assert.Equal(correctJsonString, jsonString);
        }
    }
}