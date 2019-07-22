using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using j2s.Files.Abstract;

namespace j2s.Files.Writers
{
    public class FileWriter : IFileWriter
    {
        public Task WriteAllTextAsync(string path, string content)
        {
            CreateDirectoryIfNotExists(path);
            return File.WriteAllTextAsync(path, content, Encoding.UTF8);
        }

        public void ThrowIfAnyFilesAlreadyExistsInDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            var directoryIsNotEmpty = Directory.EnumerateFileSystemEntries(directory).Any();
            if (directoryIsNotEmpty)
            {
                throw new IOException($"Directory \"{directory}\" is not empty");
            }
        }

        public Task WriteAllTextAsync(string path, string content, Encoding encoding)
        {
            CreateDirectoryIfNotExists(path);
            return File.WriteAllTextAsync(path, content, encoding);
        }
        
        internal void CreateDirectoryIfNotExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}