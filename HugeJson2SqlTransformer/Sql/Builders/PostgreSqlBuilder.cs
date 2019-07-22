using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using HugeJson2SqlTransformer.Extensions;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Sql.TableDefinition;

[assembly:InternalsVisibleTo("HugeJson2SqlTransformer.Tests")]
namespace HugeJson2SqlTransformer.Sql.Builders
{
    public class PostgreSqlBuilder : ISqlBuilder
    {
        internal List<TableColumn> TableColumns { get; } = new List<TableColumn>();
        
        public string Schema { get; private set; }
        public string Table { get; private set; }
        
        public PostgreSqlBuilder(IEnumerable<TableColumn> tableColumns)
        {
            if (tableColumns != null)
            {
                TableColumns.AddRange(tableColumns);
            }
        }

        public string BuildCreateTable()
        {
            ThrowExceptionIfTableSchemaOrNameIsIncorrect();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"create table \"{Schema}\".\"{Table}\"(");
            stringBuilder.Append(CreateTableColumnsDefinition());
            stringBuilder.Append("\n);");
            return stringBuilder.ToString();
        }

        private void ThrowExceptionIfTableSchemaOrNameIsIncorrect()
        {
            if (string.IsNullOrWhiteSpace(Schema))
                throw new ArgumentNullException(nameof(Schema));
            if (string.IsNullOrWhiteSpace(Table))
                throw new ArgumentNullException(nameof(Table));
        }

        private string CreateTableColumnsDefinition(bool onlyColumnNames = false, bool withRequired = true)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < TableColumns.Count; i++)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("    ");
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }

                var tableColumn = TableColumns[i];
                if (onlyColumnNames)
                {
                    stringBuilder.Append($"\"{tableColumn.Name}\"");
                }
                else
                {
                    stringBuilder.Append($"\"{tableColumn.Name}\" {tableColumn.Type}");

                    if (withRequired)
                    {
                        stringBuilder.Append($"{(tableColumn.Required ? " not null" : "")}");
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public string BuildInsert(string jsonItems, int? skip = null, int? limit = null)
        {
            ThrowExceptionIfTableSchemaOrNameIsIncorrect();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"insert into \"{Schema}\".\"{Table}\" (");
            stringBuilder.Append(CreateTableColumnsDefinition(onlyColumnNames: true));
            stringBuilder.Append("\n)");
            stringBuilder.Append("\nselect");
            stringBuilder.Append(CreateTableColumnsDefinition(onlyColumnNames: true));
            stringBuilder.Append("\nfrom json_to_recordset('\n");
            stringBuilder.Append(ClearJsonItemsForPostgre(jsonItems, skip, limit));
            stringBuilder.Append("\n') as x(");
            stringBuilder.Append(CreateTableColumnsDefinition(withRequired: false));
            stringBuilder.Append("\n);");
            return stringBuilder.ToString();
        }

        public void SetSchema(string schema)
        {
            Schema = schema;
        }

        public void SetTableName(string table)
        {
            Table = table;
        }

        private string ClearJsonItemsForPostgre(string jsonItems, int? skip, int? limit)
        {
            var rgx1JsonItem = new Regex(@"\{.+\}(?=(,)?(\r\n|\n))");
            var jsonItemsMatches = rgx1JsonItem.Matches(jsonItems).Select(e => e.Value).ToList();
            if (skip != null && skip >= 0)
            {
                jsonItemsMatches = jsonItemsMatches.Skip(skip.Value).ToList();
            }
            if (limit != null && limit >= 0)
            {
                jsonItemsMatches = jsonItemsMatches.Take(limit.Value).ToList();
            }
            
            var newJsonItems = jsonItemsMatches.AsJsonString();

            return //jsonItems
                newJsonItems
                    ?.Replace("\r\n", "\n")
                    ?.Replace("'", "''")
                ;
        }
    }
}