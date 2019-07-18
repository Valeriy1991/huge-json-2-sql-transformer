using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bogus;
using HugeJson2SqlTransformer.Transformers;
using Xunit;

namespace HugeJson2SqlTransformer.Tests.Unit.Transformers
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class Json2SqlTransformOptionsTests
    {
        private readonly Json2SqlTransformOptions _testModule;

        public Json2SqlTransformOptionsTests()
        {
            _testModule = new Json2SqlTransformOptions();
        }

        [Fact]
        public void Ctor_NeedSplitAllSqlDeclarationsIsTrue()
        {
            // Arrange
            // Act
            var needSplitAllSqlDeclarations = _testModule.NeedSplitAllSqlDeclarations;
            // Assert
            Assert.True(needSplitAllSqlDeclarations);
        }

        [Fact]
        public void SourceJsonFileName_ReturnOnlyFileNameWithoutExtension()
        {
            // Arrange
            _testModule.SourceJsonFilePath = @"D:\Files\example.json";
            var correctSourceJsonFileName = "example";
            // Act
            var sourceJsonFileName = _testModule.SourceJsonFileName;
            // Assert
            Assert.Equal(correctSourceJsonFileName, sourceJsonFileName);
        }

        [Fact]
        public void SourceDirectoryPath_ReturnOnlyDirectoryName()
        {
            // Arrange
            _testModule.SourceJsonFilePath = @"D:\Files\example.json";
            var correctSourceDirectoryPath = @"D:\Files";
            // Act
            var sourceDirectoryPath = _testModule.SourceDirectoryPath;
            // Assert
            Assert.Equal(correctSourceDirectoryPath, sourceDirectoryPath);
        }
    }
}