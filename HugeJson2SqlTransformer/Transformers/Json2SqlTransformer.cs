using System;
using System.Threading.Tasks;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Json;
using HugeJson2SqlTransformer.Json.Abstract;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Transformers.Abstract;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformer : IJson2SqlTransformer
    {
        private readonly IJsonFileReader _jsonFileReader;
        private readonly ISqlBuilderDirector _sqlBuilderDirector;
        private readonly ISqlBuilder _sqlBuilder;

        public Json2SqlTransformer(IJsonFileReader jsonFileReader,
            ISqlBuilderDirector sqlBuilderDirector, 
            ISqlBuilder sqlBuilder)
        {
            _jsonFileReader = jsonFileReader;
            _sqlBuilderDirector = sqlBuilderDirector;
            _sqlBuilder = sqlBuilder;
        }


        public async Task<IOutcome<string>> ExecuteAsync(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                return Outcomes.Failure<string>().WithMessage("File path is incorrect");

            try
            {
                var jsonContent = await _jsonFileReader.ReadAllTextAsync(jsonFilePath);
                await _sqlBuilderDirector.ChangeBuilder(_sqlBuilder);
                var sql = await _sqlBuilderDirector.MakeAsync(jsonContent);
                return Outcomes.Success(sql);
            }
            catch (Exception ex)
            {
                return Outcomes.Failure<string>().WithMessage(ex.Message);
            }
        }
    }
}