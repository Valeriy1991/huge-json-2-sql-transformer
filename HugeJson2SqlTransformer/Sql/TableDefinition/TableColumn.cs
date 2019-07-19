using Newtonsoft.Json;

namespace HugeJson2SqlTransformer.Sql.TableDefinition
{
    public class TableColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }

        public TableColumn(string columnName, string columnType, bool required = false)
        {
            Name = columnName;
            Type = columnType;
            Required = required;
        }
    }
}