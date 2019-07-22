using System.Text;
using System.Threading.Tasks;

namespace j2s.Files.Abstract
{
    public interface IFileWriter
    {
        void ThrowIfAnyFilesAlreadyExistsInDirectory(string directory);
        Task WriteAllTextAsync(string path, string content);
        Task WriteAllTextAsync(string path, string content, Encoding encoding);
    }
}