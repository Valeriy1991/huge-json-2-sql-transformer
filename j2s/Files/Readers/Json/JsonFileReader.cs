using System.IO;
using System.Threading.Tasks;
using j2s.Files.Abstract;

namespace j2s.Files.Readers.Json
{
    public class JsonFileReader : IFileReader
    {
        public Task<string> ReadAllTextAsync(string jsonFilePath)
        {
            return File.ReadAllTextAsync(jsonFilePath);
        }
    }
}