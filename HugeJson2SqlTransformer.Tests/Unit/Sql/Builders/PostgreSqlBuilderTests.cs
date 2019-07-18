using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Sql.Builders;
using HugeJson2SqlTransformer.Sql.TableDefinition;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Sql.Builders
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class PostgreSqlBuilderTests
    {
        private readonly Faker _faker = new Faker();
        private readonly PostgreSqlBuilder _testModule;
        private readonly List<TableColumn> _tableColumns;
        private readonly string _schema;
        private readonly string _tableName;

        public PostgreSqlBuilderTests()
        {
            _tableColumns = new List<TableColumn>()
            {
                new TableColumn("FirstName", "varchar(100)", required: true),
                new TableColumn("LastName", "varchar(100)", required: true),
                new TableColumn("IsClient", "boolean"),
                new TableColumn("Phone", "varchar(100)"),
            };
            _schema = "dbo";
            _tableName = "tableName";
            _testModule = new PostgreSqlBuilder(_tableColumns);
            _testModule.SetSchema(_schema);
            _testModule.SetTableName(_tableName);
        }

        #region Ctor

        [Fact]
        public void Ctor_TableColumnsIsEmpty()
        {
            // Arrange
            // Act
            var testModule = new PostgreSqlBuilder(null);
            // Assert
            Assert.Empty(testModule.TableColumns);
        }

        [Fact]
        public void Ctor_TableColumnsIsNotEmpty_TableColumnsHasCorrectItems()
        {
            // Arrange
            var correctTableColumns = new List<TableColumn>()
            {
                new TableColumn("FirstName", "varchar(100)", required: true),
                new TableColumn("LastName", "varchar(100)", required: true),
                new TableColumn("IsClient", "boolean"),
                new TableColumn("Phone", "varchar(100)"),
            };
            var testModule = new PostgreSqlBuilder(correctTableColumns);
            // Act
            var tableColumns = testModule.TableColumns;
            // Assert
            Assert.Contains(tableColumns, e => e.ColumnName == "FirstName");
            Assert.Contains(tableColumns, e => e.ColumnName == "LastName");
            Assert.Contains(tableColumns, e => e.ColumnName == "IsClient");
            Assert.Contains(tableColumns, e => e.ColumnName == "Phone");
        }
        
        #endregion

        #region Method: BuildCreateTable

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildCreateTable_ThrowExceptionIfTableSchemaIsNullOrEmpty(string schema)
        {
            // Arrange
            _testModule.SetSchema(schema);

            Action act = () => _testModule.BuildCreateTable();
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(_testModule.Schema), ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildCreateTable_ThrowExceptionIfTableNameIsNullOrEmpty(string tableName)
        {
            // Arrange
            _testModule.SetTableName(tableName);

            Action act = () => _testModule.BuildCreateTable();
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(_testModule.Table), ex.Message);
        }

        [Fact]
        public void BuildCreateTable_ReturnCorrectSqlStatement()
        {
            // Arrange
            var correctSqlStatement = $@"create table ""{_schema}"".""{_tableName}""(
    ""{_tableColumns[0].ColumnName}"" {_tableColumns[0].ColumnType} not null
    , ""{_tableColumns[1].ColumnName}"" {_tableColumns[1].ColumnType} not null
    , ""{_tableColumns[2].ColumnName}"" {_tableColumns[2].ColumnType}
    , ""{_tableColumns[3].ColumnName}"" {_tableColumns[3].ColumnType}
);"
                .Replace("\r\n", "\n");
            // Act
            var sqlStatement = _testModule.BuildCreateTable();
            // Assert
            Assert.Equal(correctSqlStatement, sqlStatement);
        }

        #endregion

        #region Method: BuildInsert

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildInsert_ThrowExceptionIfTableSchemaIsNullOrEmpty(string schema)
        {
            // Arrange
            _testModule.SetSchema(schema);

            var jsonItems = "some-json";
            Action act = () => _testModule.BuildInsert(jsonItems);
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(_testModule.Schema), ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BuildInsert_ThrowExceptionIfTableNameIsNullOrEmpty(string tableName)
        {
            // Arrange
            _testModule.SetTableName(tableName);

            var jsonItems = "some-json";
            Action act = () => _testModule.BuildInsert(jsonItems);
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
            Assert.Contains(nameof(_testModule.Table), ex.Message);
        }

        [Fact]
        public void BuildInsert_ReturnCorrectSqlStatement()
        {
            // Arrange
            var jsonItems = @"[
    {""firstName"": ""James"", ""lastName"": ""Bond"", ""isClient"": false, ""email"": ""james-bond@example.com""},
    {""firstName"": ""John"", ""lastName"": ""Doe"", ""isClient"": true, ""email"": ""john-doe@example.com""}
]";
            var correctSqlStatement =
                $@"insert into ""{_schema}"".""{_tableName}"" (
    ""{_tableColumns[0].ColumnName}""
    , ""{_tableColumns[1].ColumnName}""
    , ""{_tableColumns[2].ColumnName}""
    , ""{_tableColumns[3].ColumnName}""
)
select
    ""{_tableColumns[0].ColumnName}""
    , ""{_tableColumns[1].ColumnName}""
    , ""{_tableColumns[2].ColumnName}""
    , ""{_tableColumns[3].ColumnName}""
from json_to_recordset('
{jsonItems}
') as x(
    ""{_tableColumns[0].ColumnName}"" {_tableColumns[0].ColumnType} not null
    , ""{_tableColumns[1].ColumnName}"" {_tableColumns[1].ColumnType} not null
    , ""{_tableColumns[2].ColumnName}"" {_tableColumns[2].ColumnType}
    , ""{_tableColumns[3].ColumnName}"" {_tableColumns[3].ColumnType}
);"
                    .Replace("\r\n", "\n");
            // Act
            var sqlStatement = _testModule.BuildInsert(jsonItems);
            // Assert
            Assert.Equal(correctSqlStatement, sqlStatement);
        }

        [Fact]
        public void BuildInsert_JsonItemsHasSingleQuote_ReturnCorrectSqlStatementWithDoubleSingleQuotes()
        {
            // Arrange
            var jsonItems = @"[
    {""firstName"": ""James 12'/"", ""lastName"": ""Bond"", ""isClient"": false, ""email"": ""james-bond@example.com""},
    { ""firstName"": ""John"", ""lastName"": ""Doe"", ""isClient"": true, ""email"": ""john-doe@example.com"" }
]";
            var correctSqlStatement =
                $@"insert into ""{_schema}"".""{_tableName}"" (
    ""{_tableColumns[0].ColumnName}""
    , ""{_tableColumns[1].ColumnName}""
    , ""{_tableColumns[2].ColumnName}""
    , ""{_tableColumns[3].ColumnName}""
)
select
    ""{_tableColumns[0].ColumnName}""
    , ""{_tableColumns[1].ColumnName}""
    , ""{_tableColumns[2].ColumnName}""
    , ""{_tableColumns[3].ColumnName}""
from json_to_recordset('
{jsonItems.Replace("'", "''")}
') as x(
    ""{_tableColumns[0].ColumnName}"" {_tableColumns[0].ColumnType} not null
    , ""{_tableColumns[1].ColumnName}"" {_tableColumns[1].ColumnType} not null
    , ""{_tableColumns[2].ColumnName}"" {_tableColumns[2].ColumnType}
    , ""{_tableColumns[3].ColumnName}"" {_tableColumns[3].ColumnType}
);"
                    .Replace("\r\n", "\n");
            // Act
            var sqlStatement = _testModule.BuildInsert(jsonItems);
            // Assert
            Assert.Equal(correctSqlStatement, sqlStatement);
        }

        #endregion
    }
}