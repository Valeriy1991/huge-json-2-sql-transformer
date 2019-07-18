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

        public PostgreSqlBuilderTests()
        {
            _tableColumns = new List<TableColumn>()
            {
                new TableColumn("FirstName", "varchar(100)", required: true),
                new TableColumn("LastName", "varchar(100)", required: true),
                new TableColumn("IsClient", "boolean"),
                new TableColumn("Phone", "varchar(100)"),
            };
            _testModule = new PostgreSqlBuilder(_tableColumns);
        }

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

        [Fact]
        public void CreateTable_ReturnCorrectSqlStatement()
        {
            // Arrange
            var tableName = "some-table";
            var schema = "dbo";
            var correctSqlStatement = $@"create table ""{schema}"".""{tableName}""(
    ""{_tableColumns[0].ColumnName}"" {_tableColumns[0].ColumnType} not null
    , ""{_tableColumns[1].ColumnName}"" {_tableColumns[1].ColumnType} not null
    , ""{_tableColumns[2].ColumnName}"" {_tableColumns[2].ColumnType}
    , ""{_tableColumns[3].ColumnName}"" {_tableColumns[3].ColumnType}
);"
                .Replace("\r\n", "\n");
            // Act
            var sqlStatement = _testModule.CreateTable(tableName, schema);
            // Assert
            Assert.Equal(correctSqlStatement, sqlStatement);
        }

        [Fact]
        public void CreateInsert_ReturnCorrectSqlStatement()
        {
            // Arrange
            var tableName = "some-table";
            var schema = "dbo";
            var jsonItems = @"[
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
]";
            var correctSqlStatement =
                    $@"insert into ""{schema}"".""{tableName}"" (
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
            var sqlStatement = _testModule.CreateInsert(tableName, schema, jsonItems);
            // Assert
            Assert.Equal(correctSqlStatement, sqlStatement);
        }
    }
}