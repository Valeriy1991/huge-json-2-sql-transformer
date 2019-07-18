using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Files.Readers.Json;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Integration.Files.Readers.Json
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Integration")]
    public class MongoDbCompassJsonFileReaderTests
    {
        private readonly Faker _faker = new Faker();
        private readonly MongoDbCompass_1_17_0_JsonFileReader _testModule;

        public MongoDbCompassJsonFileReaderTests()
        {
            _testModule = new MongoDbCompass_1_17_0_JsonFileReader();
        }

        [Fact]
        public async Task ReadAllTextAsync_FileExists_ReturnCorrectJsonContent()
        {
            // Arrange
            var existingJsonFilePath = @".\Integration\Files\Readers\Json\mongo-db-compass-v1.17.0-json-example.json";
            // IDE "MongoDB Compass v.1.17.0" exports collections with specific JSON:
            // - without [ ] for JSON array;
            // - without "," at the end of line;
            var mongoDbCompassJsonLines = await File.ReadAllLinesAsync(existingJsonFilePath);
            var correctJsonContentStringBuilder = new StringBuilder();
            correctJsonContentStringBuilder.Append("[\n");
            var linesCount = mongoDbCompassJsonLines.Length;
            for (int i = 0; i < linesCount; i++)
            {
                correctJsonContentStringBuilder.Append(mongoDbCompassJsonLines[i]);
                if (i < linesCount - 1)
                {
                    correctJsonContentStringBuilder.Append(",\n");
                }
            }

            correctJsonContentStringBuilder.Append("\n]");
            var correctJsonContent = correctJsonContentStringBuilder.ToString();
            // Act
            var jsonContent = await _testModule.ReadAllTextAsync(existingJsonFilePath);
            // Assert
            Assert.Equal(correctJsonContent, jsonContent);
        }
    }
}