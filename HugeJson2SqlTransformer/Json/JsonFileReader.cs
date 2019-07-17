using System.IO;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Json.Abstract;

namespace HugeJson2SqlTransformer.Json
{
    public class JsonFileReader : IJsonFileReader
    {
        public Task<string> ReadAllTextAsync(string jsonFilePath)
        {
            return File.ReadAllTextAsync(jsonFilePath);
        }
    }
}