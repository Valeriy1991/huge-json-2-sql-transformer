using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Files.Abstract;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Transformers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Transformers
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class Json2SqlTransformerTests
    {
        private readonly Faker _faker = new Faker();
        private readonly Json2SqlTransformer _testModule;
        private readonly IFileReader _fileReader;
        private readonly string _sourceJsonFileName;
        private readonly Json2SqlTransformOptions _transformOptions;
        private readonly string _validJsonContent;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IFileWriter _fileWriter;

        public Json2SqlTransformerTests()
        {
            _sourceJsonFileName = "some-file";
            _transformOptions = new Json2SqlTransformOptions()
            {
                SourceJsonFilePath = _sourceJsonFileName,
            };

            _validJsonContent = @"
[
    {
	    ""firstName"": ""James"",
        ""lastName"": ""Bond"",
        ""isClient"": false,
        ""email"": ""james-bond@example.com""
    },
    {
	    ""firstName"": ""John"",
        ""lastName"": ""Doe"",
        ""isClient"": true,
        ""email"": ""john-doe@example.com""
    }
]
";
            _fileReader = Substitute.For<IFileReader>();
            _fileReader.ReadAllTextAsync($"{_sourceJsonFileName}.json").Returns(_validJsonContent);

            _sqlBuilder = Substitute.For<ISqlBuilder>();
            _fileWriter = Substitute.For<IFileWriter>();
            _testModule = new Json2SqlTransformer(_fileReader, _fileWriter, _sqlBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ExecuteAsync_SourceJsonFilePathIsNullOrEmpty_ReturnFailureWithCorrectMessage(string sourceJsonFilePath)
        {
            // Arrange
            _transformOptions.SourceJsonFilePath = sourceJsonFilePath;
            // Act
            var transformResult = await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            Assert.True(transformResult.Failure);
            Assert.Equal("Source file path is incorrect", transformResult.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_JsonFilePathIsNotNullOrEmpty_ReturnSuccess()
        {
            // Arrange
            // Act
            var transformResult = await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            Assert.True(transformResult.Success);
        }

        [Fact]
        public async Task ExecuteAsync_JsonFilePathIsCorrect_JsonFileWasRead()
        {
            // Arrange
            var correctJsonFilePath = $"{_sourceJsonFileName}.json";
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            await _fileReader.Received(1).ReadAllTextAsync(correctJsonFilePath);
        }

        [Fact]
        public async Task ExecuteAsync_SomeExceptionWasOccuredWhenJsonFileWasRead_ReturnFailureWithCorrectErrorMessage()
        {
            // Arrange
            var errorMessage = "cannot read test JSON file";
            _fileReader.ReadAllTextAsync(Arg.Any<string>()).Throws(new Exception(errorMessage));
            // Act
            var transformResult = await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            Assert.True(transformResult.Failure);
            Assert.Equal(errorMessage, transformResult.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_BuildCreateTableWasCalled()
        {
            // Arrange
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            _sqlBuilder.Received(1).BuildCreateTable();
        }

        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_BuildInsertWasCalled()
        {
            // Arrange
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            _sqlBuilder.Received(1).BuildInsert(_validJsonContent);
        }

        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_ReturnSuccess()
        {
            // Arrange
            var correctCreateTableStatement = "create table SQL";
            var correctInsertStatement = "insert to SQL table";

            _sqlBuilder.BuildCreateTable().Returns(correctCreateTableStatement);
            _sqlBuilder.BuildInsert(_validJsonContent).Returns(correctInsertStatement);
            // Act
            var transformResult = await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            Assert.True(transformResult.Success);
        }
        
        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_DatabaseSchemaWasSetUpForSqlBuilder()
        {
            // Arrange
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            _sqlBuilder.Received(1).SetSchema(_transformOptions.TableSchema);
        }

        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_DatabaseTableNameWasSetUpForSqlBuilder()
        {
            // Arrange
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            _sqlBuilder.Received(1).SetTableName(_transformOptions.TableName);
        }

        [Fact]
        public async Task ExecuteAsync_JsonFileHasValidContent_CreateTableAndInsertStatementsWasBuiltSeparately()
        {
            // Arrange
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            _sqlBuilder.Received(1).BuildCreateTable();
            _sqlBuilder.Received(1).BuildInsert(_validJsonContent);
        }

        [Fact]
        public async Task ExecuteAsync_FileWithSqlStatementOfCreateTableWasCreated()
        {
            // Arrange
            var correctFilePath = $"001-{_sourceJsonFileName}-create-table.sql";
            var correctCreateTableSqlStatement = "create table SQL statement";
            _sqlBuilder.BuildCreateTable().Returns(correctCreateTableSqlStatement);
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            await _fileWriter.Received(1).WriteAllTextAsync(correctFilePath, correctCreateTableSqlStatement);
        }

        [Fact]
        public async Task ExecuteAsync_FileWithSqlStatementOfInsertValuesWasCreated()
        {
            // Arrange
            var correctFilePath = $"002-{_sourceJsonFileName}-insert-values.sql";
            var correctInsertValuesSqlStatement = "create table SQL statement";
            _sqlBuilder.BuildInsert(_validJsonContent).Returns(correctInsertValuesSqlStatement);
            // Act
            await _testModule.ExecuteAsync(_transformOptions);
            // Assert
            await _fileWriter.Received(1).WriteAllTextAsync(correctFilePath, correctInsertValuesSqlStatement);
        }
    }
}