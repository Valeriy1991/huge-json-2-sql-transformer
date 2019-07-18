using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Files.Abstract
{
    public interface IFileReader
    {
        Task<string> ReadAllTextAsync(string jsonFilePath);
    }
}