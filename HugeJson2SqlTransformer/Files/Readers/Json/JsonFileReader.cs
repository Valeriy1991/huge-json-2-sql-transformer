using System.IO;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Files.Abstract;

namespace HugeJson2SqlTransformer.Files.Readers.Json
{
    public class JsonFileReader : IFileReader
    {
        public Task<string> ReadAllTextAsync(string jsonFilePath)
        {
            return File.ReadAllTextAsync(jsonFilePath);
        }
    }
}