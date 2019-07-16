using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Json.Abstract
{
    public interface IJsonFileReader
    {
        Task<string> ReadAllTextAsync(string jsonFilePath);
    }
}