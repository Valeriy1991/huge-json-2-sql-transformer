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
        private string _path;

        public FileWriterTests()
        {
            _testModule = new FileWriter();
            _path = @"created-file.txt";
        }

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

        [Fact]
        public async Task WriteAllTextAsync_DirectoryCreatedSuccessfullyIfNotExists()
        {
            // Arrange
            var content = "Some file content";
            var directory = "test-dir";
            var path = Path.Combine(directory, _path);
            // Act
            await _testModule.WriteAllTextAsync(path, content);
            // Assert
            Directory.Exists(_path);

            // Post-test actions:
            File.Delete(path);
            Directory.Delete(directory);
        }

        public void Dispose()
        {
            File.Delete(_path);
        }
    }
}