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
        private SqlBuilderDirector _testModule;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly string _validJsonContent;
        private readonly string _tableName;
        private readonly string _schema;

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
            _sqlBuilder.CreateTable(_tableName, _schema).Returns("create table");
            _sqlBuilder.CreateManyInserts(_tableName, _schema).Returns("create many inserts");

            _tableName = "some-table";
            _schema = "dbo";
            _testModule = new SqlBuilderDirector(_sqlBuilder, _tableName, _schema);
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
                _sqlBuilder.CreateTable(_tableName, _schema);
                _sqlBuilder.CreateManyInserts(_tableName, _schema);
            });
        }

        [Fact]
        public async Task MakeAsync_JsonContentIsCorrect_ReturnCorrectSqlStatement()
        {
            // Arrange
            var createTableSqlStatement = "create table SQL-statement";
            _sqlBuilder.CreateTable(_tableName, _schema).Returns(createTableSqlStatement);

            var manyInsertsSqlStatement = $@"
insert 1;
insert 2;
insert 3;
...
insert 10000;
";
            _sqlBuilder.CreateManyInserts(_tableName, _schema).Returns(manyInsertsSqlStatement);

            var correctSqlStatement = $"{createTableSqlStatement}\n{manyInsertsSqlStatement}";
            // Act
            var sql = await _testModule.MakeAsync(_validJsonContent);
            // Assert
            Assert.Equal(correctSqlStatement, sql);
        }

        [Fact]
        public void Ctor_SqlBuilderIsCorrect()
        {
            // Arrange
            var sqlBuilder = Substitute.For<ISqlBuilder>();
            // Act
            _testModule = new SqlBuilderDirector(sqlBuilder, _tableName, _schema);
            // Assert
            Assert.Equal(sqlBuilder, _testModule.SqlBuilder);
        }

        [Fact]
        public async Task ChangeBuilder_InnerBuilderWasReallyChanged()
        {
            // Arrange
            var sqlBuilder = Substitute.For<ISqlBuilder>();
            // Act
            await _testModule.ChangeBuilder(sqlBuilder);
            // Assert
            Assert.Equal(sqlBuilder, _testModule.SqlBuilder);
        }

        [Fact]
        public async Task ChangeBuilder_ResetInnerStringBuilder()
        {
            // Arrange
            var sqlBuilder = Substitute.For<ISqlBuilder>();
            var stringBuilderBeforeChangeBuilder = _testModule.StringSqlBuilder;
            // Act
            await _testModule.ChangeBuilder(sqlBuilder);
            // Assert
            var stringBuilderAfterChangeBuilder = _testModule.StringSqlBuilder;
            Assert.NotEqual(stringBuilderBeforeChangeBuilder, stringBuilderAfterChangeBuilder);
        }
    }
}