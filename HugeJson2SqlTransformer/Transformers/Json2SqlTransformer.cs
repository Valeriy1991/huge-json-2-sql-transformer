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
        private readonly ISqlBuilder _sqlBuilder;

        public Json2SqlTransformer(IFileReader jsonFileReader,
            IFileWriter fileWriter,
            ISqlBuilder sqlBuilder)
        {
            _jsonFileReader = jsonFileReader;
            _fileWriter = fileWriter;
            _sqlBuilder = sqlBuilder;
        }


        public async Task<IOutcome> ExecuteAsync(Json2SqlTransformOptions transformOptions)
        {
            if (string.IsNullOrWhiteSpace(transformOptions.SourceJsonFile))
                return Outcomes.Failure().WithMessage("Source file path is incorrect");
            if (string.IsNullOrWhiteSpace(transformOptions.TargetSqlFile))
                return Outcomes.Failure().WithMessage("Target file path is incorrect");

            try
            {
                var jsonContent = await _jsonFileReader.ReadAllTextAsync(transformOptions.SourceJsonFile);
                var sql = CreateSqlStatement(transformOptions, jsonContent);
                await _fileWriter.WriteAllTextAsync(transformOptions.TargetSqlFile, sql);
                return Outcomes.Success(sql);
            }
            catch (Exception ex)
            {
                return Outcomes.Failure().WithMessage(ex.Message);
            }
        }

        private string CreateSqlStatement(Json2SqlTransformOptions transformOptions, string jsonContent)
        {
            _sqlBuilder.SetSchema(transformOptions.TableSchema);
            _sqlBuilder.SetTableName(transformOptions.TableName);

            var createTableStatement = _sqlBuilder.BuildCreateTable();
            var insertStatement = _sqlBuilder.BuildInsert(jsonContent);

            var sql = createTableStatement + "\n" + insertStatement;
            return sql;
        }
    }
}