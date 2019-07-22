using System.Threading.Tasks;
using Ether.Outcomes;

namespace j2s.Transformers.Abstract
{
    public interface IJson2SqlTransformer
    {
        Task<IOutcome> ExecuteAsync(Json2SqlTransformOptions transformOptions);
    }
}