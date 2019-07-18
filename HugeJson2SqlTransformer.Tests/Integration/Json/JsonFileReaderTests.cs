using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Json;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Integration.Json
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Integration")]
    public class JsonFileReaderTests
    {
        private readonly JsonFileReader _testModule;

        public JsonFileReaderTests()
        {
            _testModule = new JsonFileReader();
        }

        [Fact]
        public async Task ReadAllTextAsync_FileNotFound_ThrowException()
        {
            // Arrange
            var nonExistingJsonFilePath = "non-existing-file.json";
            Func<Task> act = () => _testModule.ReadAllTextAsync(nonExistingJsonFilePath);
            // Act
            var ex = await Record.ExceptionAsync(act);
            // Assert
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact]
        public async Task ReadAllTextAsync_JsonFileExists_ReturnJsonContent()
        {
            // Arrange
            var existingJsonFilePath = ".\\Integration\\Json\\json-example.json";
            var correctJsonContent = await File.ReadAllTextAsync(existingJsonFilePath);
            // Act
            var jsonContent = await _testModule.ReadAllTextAsync(existingJsonFilePath);
            // Assert
            Assert.Equal(correctJsonContent, jsonContent);
        }
    }
}