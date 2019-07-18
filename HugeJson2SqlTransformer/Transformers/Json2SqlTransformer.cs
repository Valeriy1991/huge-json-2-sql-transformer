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
        private readonly IFileReader _fileReader;
        private readonly IFileWriter _fileWriter;
        private readonly ISqlBuilder _sqlBuilder;

        public Json2SqlTransformer(IFileReader jsonFileReader,
            IFileWriter fileWriter,
            ISqlBuilder sqlBuilder)
        {
            _fileReader = jsonFileReader;
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
                InitSqlBuilder(transformOptions);

                var jsonContent = await ReadJsonFile(transformOptions);
                await CreateFileWithCreateTableSqlStatement(transformOptions);
                await CreateFileWithInsertSqlStatements(transformOptions, jsonContent);

                return Outcomes.Success();
            }
            catch (Exception ex)
            {
                return Outcomes.Failure().WithMessage(ex.Message);
            }
        }

        private Task<string> ReadJsonFile(Json2SqlTransformOptions transformOptions)
        {
            var jsonFilePath = $"{transformOptions.SourceJsonFile}.json";
            return _fileReader.ReadAllTextAsync(jsonFilePath);
        }

        private void InitSqlBuilder(Json2SqlTransformOptions transformOptions)
        {
            _sqlBuilder.SetSchema(transformOptions.TableSchema);
            _sqlBuilder.SetTableName(transformOptions.TableName);
        }

        private Task CreateFileWithCreateTableSqlStatement(Json2SqlTransformOptions transformOptions)
        {
            var targetSqlFilePath = $"001-{transformOptions.TargetSqlFile}-create-table.sql";

            var createTableStatement = _sqlBuilder.BuildCreateTable();
            return _fileWriter.WriteAllTextAsync(targetSqlFilePath, createTableStatement);
        }

        private Task CreateFileWithInsertSqlStatements(Json2SqlTransformOptions transformOptions, string jsonContent)
        {
            var targetSqlFilePath = $"002-{transformOptions.TargetSqlFile}-insert-values.sql";
            var insertStatement = _sqlBuilder.BuildInsert(jsonContent);
            return _fileWriter.WriteAllTextAsync(targetSqlFilePath, insertStatement);
        }
    }
}