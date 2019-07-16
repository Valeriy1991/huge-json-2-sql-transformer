using System.Threading.Tasks;
using Ether.Outcomes;

namespace HugeJson2SqlTransformer.Validators.Abstract
{
    public interface IJsonFileValidator
    {
        Task<IOutcome> ValidateAsync(string jsonContent);
    }
}