namespace HugeJson2SqlTransformer.Sql.Abstract
{
    public interface ISqlBuilder
    {
        string CreateTable(string tableName, string schema);
        string CreateManyInserts(string tableName, string schema);
    }
}