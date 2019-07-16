using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Sql.Abstract
{
    public interface ISqlBuilderDirector
    {
        Task<string> MakeAsync(string jsonContent);
    }
}