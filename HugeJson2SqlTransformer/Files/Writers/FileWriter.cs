using System.IO;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Files.Abstract;

namespace HugeJson2SqlTransformer.Files.Writers
{
    public class FileWriter : IFileWriter
    {
        public Task WriteAllTextAsync(string path, string content)
        {
            CreateDirectoryIfNotExists(path);
            return File.WriteAllTextAsync(path, content, Encoding.UTF8);
        }

        public Task WriteAllTextAsync(string path, string content, Encoding encoding)
        {
            CreateDirectoryIfNotExists(path);
            return File.WriteAllTextAsync(path, content, encoding);
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}