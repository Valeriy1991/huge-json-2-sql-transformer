using System;
using System.Threading.Tasks;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Json;
using HugeJson2SqlTransformer.Json.Abstract;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Transformers.Abstract;
using HugeJson2SqlTransformer.Validators.Abstract;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformer : IJson2SqlTransformer
    {
        private readonly IJsonFileReader _jsonFileReader;
        private readonly IJsonFileValidator _jsonFileValidator;
        private readonly ISqlBuilderDirector _sqlBuilderDirector;

        public Json2SqlTransformer(
            IJsonFileReader jsonFileReader, 
            IJsonFileValidator jsonFileValidator,
            ISqlBuilderDirector sqlBuilderDirector)
        {
            _jsonFileReader = jsonFileReader;
            _jsonFileValidator = jsonFileValidator;
            _sqlBuilderDirector = sqlBuilderDirector;
        }


        public async Task<IOutcome<string>> ExecuteAsync(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                return Outcomes.Failure<string>().WithMessage("File path is incorrect");

            try
            {
                var jsonContent = await _jsonFileReader.ReadAllTextAsync(jsonFilePath);
                var validationResult = await _jsonFileValidator.ValidateAsync(jsonContent);
                if (validationResult.Failure)
                    return Outcomes.Failure<string>()
                        .WithMessage($"File has incorrect JSON: {validationResult.ToString()}");

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