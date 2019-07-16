using System;
using System.Threading.Tasks;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Json;
using HugeJson2SqlTransformer.Json.Abstract;
using HugeJson2SqlTransformer.Transformers.Abstract;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2PostgreSqlTransformer : IJson2SqlTransformer
    {
        private readonly IJsonFileReader _jsonFileReader;

        public Json2PostgreSqlTransformer(IJsonFileReader jsonFileReader)
        {
            _jsonFileReader = jsonFileReader;
        }


        public async Task<IOutcome<string>> Execute(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                return Outcomes.Failure<string>().WithMessage("File path is incorrect");

            try
            {
                var jsonContent = await _jsonFileReader.ReadAllTextAsync(jsonFilePath);

                return Outcomes.Success<string>();
            }
            catch (Exception ex)
            {
                return Outcomes.Failure<string>().WithMessage(ex.Message);
            }
        }
    }
}