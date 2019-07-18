using System;
using System.Threading.Tasks;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Files.Abstract;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Transformers.Abstract;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformer : IJson2SqlTransformer
    {
        private readonly IFileReader _jsonFileReader;
        private readonly IFileWriter _fileWriter;
        private readonly ISqlBuilderDirector _sqlBuilderDirector;
        private readonly ISqlBuilder _sqlBuilder;

        public Json2SqlTransformer(IFileReader jsonFileReader,
            IFileWriter fileWriter,
            ISqlBuilderDirector sqlBuilderDirector,
            ISqlBuilder sqlBuilder)
        {
            _jsonFileReader = jsonFileReader;
            _fileWriter = fileWriter;
            _sqlBuilderDirector = sqlBuilderDirector;
            _sqlBuilder = sqlBuilder;
        }


        public async Task<IOutcome> ExecuteAsync(string sourceJsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceJsonFilePath))
                return Outcomes.Failure().WithMessage("File path is incorrect");

            try
            {
                var jsonContent = await _jsonFileReader.ReadAllTextAsync(sourceJsonFilePath);
                await _sqlBuilderDirector.ChangeBuilder(_sqlBuilder);
                var sql = await _sqlBuilderDirector.MakeAsync(jsonContent);
                await _fileWriter.WriteAllTextAsync(sql);
                return Outcomes.Success(sql);
            }
            catch (Exception ex)
            {
                return Outcomes.Failure().WithMessage(ex.Message);
            }
        }
    }
}