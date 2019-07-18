using System.Collections.Generic;
using HugeJson2SqlTransformer.Sql.TableDefinition;

namespace HugeJson2SqlTransformer.Sql.Abstract
{
    public interface ISqlBuilder
    {
        string Schema { get; }
        string Table { get; }
        string BuildCreateTable();
        string BuildInsert(string jsonArray, int? skip = null, int? limit = null);
        void SetSchema(string schema);
        void SetTableName(string tableName);
    }
}