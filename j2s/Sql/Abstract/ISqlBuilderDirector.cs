using System.Threading.Tasks;

namespace j2s.Sql.Abstract
{
    public interface ISqlBuilderDirector
    {
        Task ChangeBuilder(ISqlBuilder sqlBuilder);
        Task<string> MakeAsync(string jsonContent);
    }
}