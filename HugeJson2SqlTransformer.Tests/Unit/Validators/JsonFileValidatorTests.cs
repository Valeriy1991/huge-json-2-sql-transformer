using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Validators;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Validators
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class JsonFileValidatorTests
    {
        private readonly Faker _faker = new Faker();
        private readonly JsonFileValidator _testModule;
        private readonly string _incorrectJson;
        private readonly string _correctJson;
        private readonly string _jsonSchema;

        public JsonFileValidatorTests()
        {
            _jsonSchema = @"{
  'description': 'A person',
  'type': 'object',
  'properties': {
    'firstName': {'type': 'string'},
    'lastName': {'type': 'string'},
    'isClient': {'type': 'boolean'},
    'phone': {'type': 'string'}
  }
}
";
            _incorrectJson = @"
{""test"": 123 4231\asd ; }
";
            _correctJson = @"
[
    {
        ""firstName"": ""James"",
        ""lastName"": ""Bond"",
        ""isClient"": false,
        ""phone"": ""james-bond@example.com""
    },
    {
        ""firstName"": ""John"",
        ""lastName"": ""Doe"",
        ""isClient"": true,
        ""phone"": ""john-doe@example.com""
    }
]
";
            _testModule = new JsonFileValidator();
        }

        [Fact]
        public async Task ValidateAsync_JsonIsIncorrect_ReturnFailure()
        {
            // Arrange
            // Act
            var validationResult = await _testModule.ValidateAsync(_jsonSchema, _incorrectJson);
            // Assert
            Assert.True(validationResult.Failure);
        }
        [Fact]
        public async Task ValidateAsync_JsonIsCorrect_ReturnFailure()
        {
            // Arrange
            // Act
            var validationResult = await _testModule.ValidateAsync(_jsonSchema, _correctJson);
            // Assert
            Assert.True(validationResult.Success);
        }
    }
}