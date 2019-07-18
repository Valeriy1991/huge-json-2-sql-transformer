using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Files.Writers;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Integration.Files.Writers
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Integration")]
    public class FileWriterTests : IDisposable
    {
        private readonly FileWriter _testModule;
        private readonly string _fileName;
        private readonly string _path;

        public FileWriterTests()
        {
            _testModule = new FileWriter();
            _fileName = "created-file";
            _path = $"{_fileName}.txt";
        }

        #region Method: CreateDirectoryIfNotExists

        [Fact]
        public void CreateDirectoryIfNotExists_PathToFileHasNotDirectory_DirectoryWillNotBeCreated()
        {
            // Arrange
            var pathToFileWithoutDirectory = "some-file-without-directory.txt";
            var directory = Path.GetDirectoryName(pathToFileWithoutDirectory);
            // Act
            _testModule.CreateDirectoryIfNotExists(pathToFileWithoutDirectory);
            // Assert
            var directoryExists = Directory.Exists(directory);
            Assert.False(directoryExists);
        }

        [Fact]
        public void CreateDirectoryIfNotExists_PathIsNotNullOrEmpty_DirectoryWillBeCreated()
        {
            // Arrange
            var newDirectory = "new-directory";
            var pathToFileWithDirectory = Path.Combine(newDirectory, "some-file.txt");
            // Act
            _testModule.CreateDirectoryIfNotExists(pathToFileWithDirectory);
            // Assert
            var directoryExists = Directory.Exists(newDirectory);
            Assert.True(directoryExists);
        }

        #endregion

        #region Method: WriteAllTextAsync

        [Fact]
        public async Task WriteAllTextAsync_EncodingIsDefault_FileCreatedSuccessfullyWithUTF8Encoding()
        {
            // Arrange
            var content = "Some file content";
            // Act
            await _testModule.WriteAllTextAsync(_path, content);
            // Assert
            Assert.True(File.Exists(_path));
            var createdFileEncoding = GetEncodingOfFile(_path);
            Assert.Equal(Encoding.UTF8, createdFileEncoding);
        }

        [Fact]
        public async Task WriteAllTextAsync_EncodingWasSetAsParameter_FileCreatedSuccessfullyWithCorrectEncoding()
        {
            // Arrange
            var content = "Some file content";
            var encoding = Encoding.Unicode;
            // Act
            await _testModule.WriteAllTextAsync(_path, content, encoding);
            // Assert
            Assert.True(File.Exists(_path));
            var createdFileEncoding = GetEncodingOfFile(_path);
            Assert.Equal(encoding, createdFileEncoding);
        }

        [Fact]
        public async Task WriteAllTextAsync_DirectoryCreatedSuccessfullyIfNotExists()
        {
            // Arrange
            var content = "Some file content";
            var directory = "test-dir-1";
            var path = Path.Combine(directory, _path);
            // Act
            await _testModule.WriteAllTextAsync(path, content);
            // Assert
            Directory.Exists(_path);
            // Post-assert
            File.Delete(path);
            Directory.Delete(directory);
        }

        [Fact]
        public void ThrowIfAnyFilesAlreadyExistsInDirectory_DirectoryAlreadyExistsAndHasSomeFiles_ThrowExceptionWithCorrectMessage()
        {
            // Arrange
            var content = "Some file content";
            var directory = "test-dir-2";
            var path = Path.Combine(directory, _path);
            var filePath1 = Path.Combine(directory, $"{_fileName}-1.txt");
            var filePath2 = Path.Combine(directory, $"{_fileName}-2.txt");
            Directory.CreateDirectory(directory);
            File.WriteAllText(filePath1, content);
            File.WriteAllText(filePath2, content);

            Action act = () => _testModule.ThrowIfAnyFilesAlreadyExistsInDirectory(directory);
            // Act
            var ex = Record.Exception(act);
            // Assert
            Assert.IsType<IOException>(ex);
            Assert.Equal($"Directory \"{directory}\" is not empty", ex.Message);
            // Post-assert
            File.Delete(path);
            File.Delete(filePath1);
            File.Delete(filePath2);
            Directory.Delete(directory);
        }

        #endregion

        private Encoding GetEncodingOfFile(string filePath)
        {
            Encoding createdFileEncoding;
            using (var reader = new StreamReader(filePath))
            {
                reader.Peek();
                createdFileEncoding = reader.CurrentEncoding;
            }

            return createdFileEncoding;
        }

        public void Dispose()
        {
            File.Delete(_path);
        }
    }
}