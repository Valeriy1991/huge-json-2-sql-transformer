using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Files.Abstract
{
    public interface IFileWriter
    {
        Task WriteAllTextAsync(string text);
    }
}