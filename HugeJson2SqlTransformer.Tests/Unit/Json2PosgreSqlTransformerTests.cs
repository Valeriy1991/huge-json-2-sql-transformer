using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Json;
using HugeJson2SqlTransformer.Json.Abstract;
using HugeJson2SqlTransformer.Transformers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class Json2PostgreSqlTransformerTests
    {
        private readonly Faker _faker = new Faker();
        private readonly Json2PostgreSqlTransformer _testModule;
        private readonly IJsonFileReader _jsonFileReader;
        private string _jsonFilePath;

        public Json2PostgreSqlTransformerTests()
        {
            _jsonFilePath = "some-file.json";

            _jsonFileReader = Substitute.For<IJsonFileReader>();
            _testModule = new Json2PostgreSqlTransformer(_jsonFileReader);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Execute_JsonFilePathIsNullOrEmpty_ReturnFailureWithCorrectMessage(string jsonFilePath)
        {
            // Arrange
            // Act
            var transformResult = await _testModule.Execute(jsonFilePath);
            // Assert
            Assert.True(transformResult.Failure);
            Assert.Equal("File path is incorrect", transformResult.ToString());
        }

        [Fact]
        public async Task Execute_JsonFilePathIsNotNullOrEmpty_ReturnSuccess()
        {
            // Arrange
            // Act
            var transformResult = await _testModule.Execute(_jsonFilePath);
            // Assert
            Assert.True(transformResult.Success);
        }

        [Fact]
        public async Task Execute_JsonFileWasRead()
        {
            // Arrange
            // Act
            await _testModule.Execute(_jsonFilePath);
            // Assert
            await _jsonFileReader.Received(1).ReadAllTextAsync(_jsonFilePath);
        }

        [Fact]
        public async Task Execute_SomeExceptionWasOccuredWhenJsonFileWasRead()
        {
            // Arrange
            var errorMessage = "cannot read test JSON file";
            _jsonFileReader.ReadAllTextAsync(Arg.Any<string>()).Throws(new Exception(errorMessage));
            // Act
            var transformResult = await _testModule.Execute(_jsonFilePath);
            // Assert
            Assert.True(transformResult.Failure);
            Assert.Equal(errorMessage, transformResult.ToString());
        }
    }
}