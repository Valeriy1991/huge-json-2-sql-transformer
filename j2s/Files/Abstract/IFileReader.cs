using System.Threading.Tasks;

namespace j2s.Files.Abstract
{
    public interface IFileReader
    {
        Task<string> ReadAllTextAsync(string jsonFilePath);
    }
}