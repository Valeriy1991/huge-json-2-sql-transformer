using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Sql.Abstract
{
    public interface ISqlBuilderDirector
    {
        Task ChangeBuilder(ISqlBuilder sqlBuilder);
        Task<string> MakeAsync(string jsonContent);
    }
}