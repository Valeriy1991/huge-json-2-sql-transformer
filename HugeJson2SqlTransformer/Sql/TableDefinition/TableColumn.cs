namespace HugeJson2SqlTransformer.Sql.TableDefinition
{
    public class TableColumn
    {
        public string ColumnName { get; }
        public string ColumnType { get; }
        public bool Required { get; }

        public TableColumn(string columnName, string columnType, bool required = false)
        {
            ColumnName = columnName;
            ColumnType = columnType;
            Required = required;
        }
    }
}