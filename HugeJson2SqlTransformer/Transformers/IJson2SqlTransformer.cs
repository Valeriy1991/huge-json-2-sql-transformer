using System.Threading.Tasks;
using Ether.Outcomes;

namespace HugeJson2SqlTransformer.Transformers
{
    public interface IJson2SqlTransformer
    {
        Task<IOutcome<string>> Execute(string jsonFilePath);
    }
}