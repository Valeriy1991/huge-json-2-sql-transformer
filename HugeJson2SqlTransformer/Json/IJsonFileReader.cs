using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Json
{
    public interface IJsonFileReader
    {
        Task<string> ReadAllTextAsync(string jsonFilePath);
    }
}