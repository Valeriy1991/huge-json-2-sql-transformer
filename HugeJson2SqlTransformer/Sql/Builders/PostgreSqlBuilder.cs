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
            for (int i = 0; i < TableColumns.Count; i++)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("    ");
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }

                var tableColumn = TableColumns[i];
                stringBuilder.Append(
                    $"\"{tableColumn.ColumnName}\" {tableColumn.ColumnType}{(tableColumn.Required ? " not null" : "")}");
            }

            stringBuilder.Append("\n");
            stringBuilder.Append(");");
            return stringBuilder.ToString();
        }

        public string CreateManyInserts(string tableName, string schema)
        {
            throw new System.NotImplementedException();
        }
    }
}