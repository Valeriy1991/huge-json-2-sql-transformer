using System.Text;
using System.Threading.Tasks;

namespace HugeJson2SqlTransformer.Files.Abstract
{
    public interface IFileWriter
    {
        Task WriteAllTextAsync(string path, string content);
        Task WriteAllTextAsync(string path, string content, Encoding encoding);
    }
}