using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Sql;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Sql.TableDefinition;
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
            _sqlBuilder = Substitute.For<ISqlBuilder>();
            _sqlBuilder.CreateTable(_tableName, _schema).Returns("create table");
            _sqlBuilder.CreateInsert(_tableName, _schema, _validJsonContent)
                .Returns("create many inserts");

            _tableName = "some-table";
            _schema = "dbo";
            _testModule = new SqlBuilderDirector(_tableName, _schema);
            _testModule.ChangeBuilder(_sqlBuilder).GetAwaiter().GetResult();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task MakeAsync_JsonContentIsNullOrEmpty_ThrowExceptionWithCorrectMessage(string jsonContent)
        {
            // Arrange
            Func<Task<string>> act = () => _testModule.MakeAsync(jsonContent);
            // Act
            var ex = await Record.ExceptionAsync(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains("jsonContent", ex.Message);
        }

        [Fact]
        public async Task MakeAsync_SqlBuilderWasNotInit_ThrowExceptionWithCorrectMessage()
        {
            // Arrange
            await _testModule.ChangeBuilder(null);
            Func<Task<string>> act = () => _testModule.MakeAsync(_validJsonContent);
            // Act
            var ex = await Record.ExceptionAsync(act);
            // Assert
            Assert.IsType<NullReferenceException>(ex);
            Assert.Equal("\"SqlBuilder\" is null", ex.Message);
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
                _sqlBuilder.CreateInsert(_tableName, _schema, _validJsonContent);
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
            _sqlBuilder.CreateInsert(_tableName, _schema, _validJsonContent).Returns(manyInsertsSqlStatement);

            var correctSqlStatement = $"{createTableSqlStatement}\n{manyInsertsSqlStatement}";
            // Act
            var sql = await _testModule.MakeAsync(_validJsonContent);
            // Assert
            Assert.Equal(correctSqlStatement, sql);
        }

        #region Ctor

        [Fact]
        public void Ctor_SqlBuilderIsNull()
        {
            // Arrange
            // Act
            _testModule = new SqlBuilderDirector(_tableName, _schema);
            // Assert
            Assert.Null(_testModule.SqlBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Ctor_TableNameIsNullOrEmpty_ThrowExceptionWithCorrectMessage(string tableName)
        {
            // Arrange
            Action act = () => new SqlBuilderDirector(tableName, _schema);
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(tableName), ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Ctor_SchemaIsNullOrEmpty_ThrowExceptionWithCorrectMessage(string schema)
        {
            // Arrange
            Action act = () => new SqlBuilderDirector(_tableName, schema);
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(schema), ex.Message);
        }

        #endregion

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