using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Files.Writers;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Integration.Files.Writers
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Integration")]
    public class FileWriterTests : IDisposable
    {
        private readonly Faker _faker = new Faker();
        private readonly FileWriter _testModule;
        private readonly string _path;

        public FileWriterTests()
        {
            _testModule = new FileWriter();
            _path = @"created-file.txt";
        }

        [Fact]
        public async Task WriteAllTextAsync_FileCreatedAfterWritingFinished()
        {
            // Arrange
            string content = "Some file content";
            // Act
            await _testModule.WriteAllTextAsync(_path, content);
            // Assert
            Assert.True(File.Exists(_path));
            Encoding createdFileEncoding;
            using (var reader = new StreamReader(_path))
            {
                reader.Peek();
                createdFileEncoding = reader.CurrentEncoding;
            }

            Assert.Equal(Encoding.UTF8, createdFileEncoding);
        }

        public void Dispose()
        {
            File.Delete(_path);
        }
    }
}