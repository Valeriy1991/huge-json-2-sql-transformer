namespace HugeJson2SqlTransformer.Sql.Abstract
{
    public interface ISqlBuilder
    {
        string CreateTable();
        string CreateManyInserts();
    }
}