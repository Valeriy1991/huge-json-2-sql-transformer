using System.Collections.Generic;
using System.Text;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Sql.TableDefinition;

namespace HugeJson2SqlTransformer.Sql.Builders
{
    public class PostgreSqlBuilder : ISqlBuilder
    {
        internal List<TableColumn> TableColumns { get; } = new List<TableColumn>();

        public PostgreSqlBuilder(IEnumerable<TableColumn> tableColumns)
        {
            if (tableColumns != null)
            {
                TableColumns.AddRange(tableColumns);
            }
        }

        public string CreateTable(string tableName, string schema)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"create table \"{schema}\".\"{tableName}\"(");
            stringBuilder.Append(CreateTableColumnsDefinition());
            stringBuilder.Append("\n);");
            return stringBuilder.ToString();
        }

        private string CreateTableColumnsDefinition(bool onlyColumnNames = false)
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
                    stringBuilder.Append($"\"{tableColumn.ColumnName}\"");
                }
                else
                {
                    stringBuilder.Append(
                        $"\"{tableColumn.ColumnName}\" {tableColumn.ColumnType}{(tableColumn.Required ? " not null" : "")}");
                }
            }

            return stringBuilder.ToString();
        }

        public string CreateInsert(string tableName, string schema, string jsonItems)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"insert into \"{schema}\".\"{tableName}\" (");
            stringBuilder.Append(CreateTableColumnsDefinition(onlyColumnNames: true));
            stringBuilder.Append("\n)");
            stringBuilder.Append("\nselect");
            stringBuilder.Append(CreateTableColumnsDefinition(onlyColumnNames: true));
            stringBuilder.Append("\nfrom json_to_recordset('\n");
            stringBuilder.Append(jsonItems?.Replace("\r\n", "\n"));
            stringBuilder.Append("\n') as x(");
            stringBuilder.Append(CreateTableColumnsDefinition());
            stringBuilder.Append("\n);");
            return stringBuilder.ToString();
        }
    }
}