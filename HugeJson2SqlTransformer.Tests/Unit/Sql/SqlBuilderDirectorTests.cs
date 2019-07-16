using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Sql;
using HugeJson2SqlTransformer.Sql.Abstract;
using NSubstitute;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Sql
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class SqlBuilderDirectorTests
    {
        private readonly Faker _faker = new Faker();
        private readonly SqlBuilderDirector _testModule;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly string _validJsonContent;

        public SqlBuilderDirectorTests()
        {
            _validJsonContent = @"
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
            _sqlBuilder = Substitute.For<ISqlBuilder>();
            _testModule = new SqlBuilderDirector(_sqlBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task MakeAsync_JsonContentIsNullOrEmpty_ThrowArgumentNullException(string jsonContent)
        {
            // Arrange
            Func<Task<string>> act = () => _testModule.MakeAsync(jsonContent);
            // Act
            var ex = await Record.ExceptionAsync(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public async Task MakeAsync_JsonContentIsCorrect_CreateCorrectSqlStatements()
        {
            // Arrange
            // Act
            await _testModule.MakeAsync(_validJsonContent);
            // Assert
            Received.InOrder(() =>
            {
                _sqlBuilder.CreateTable();
                _sqlBuilder.CreateManyInserts();
            });
        }

        [Fact]
        public async Task MakeAsync_JsonContentIsCorrect_ReturnCorrectSqlStatement()
        {
            // Arrange
            var createTableSqlStatement = "create table SQL-statement";
            _sqlBuilder.CreateTable().Returns(createTableSqlStatement);

            var manyInsertsSqlStatement = $@"
insert 1;
insert 2;
insert 3;
...
insert 10000;
";
            _sqlBuilder.CreateManyInserts().Returns(manyInsertsSqlStatement);

            var correctSqlStatement = $"{createTableSqlStatement}\n{manyInsertsSqlStatement}";
            // Act
            var sql = await _testModule.MakeAsync(_validJsonContent);
            // Assert
            Assert.Equal(correctSqlStatement, sql);
        }
    }
}