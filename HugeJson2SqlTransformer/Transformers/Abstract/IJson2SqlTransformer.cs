using System.Threading.Tasks;
using Ether.Outcomes;

namespace HugeJson2SqlTransformer.Transformers.Abstract
{
    public interface IJson2SqlTransformer
    {
        Task<IOutcome> ExecuteAsync(string sourceJsonFilePath);
    }
}