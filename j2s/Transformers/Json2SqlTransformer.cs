using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ether.Outcomes;
using j2s.Files.Abstract;
using j2s.Sql.Abstract;
using j2s.Transformers.Abstract;

namespace j2s.Transformers
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
            if (string.IsNullOrWhiteSpace(transformOptions.SourceJsonFilePath))
                return Outcomes.Failure().WithMessage("Source file path is incorrect");

            try
            {
                ThrowIfAnyFilesAlreadyExistsInDirectory(transformOptions);
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

        private void ThrowIfAnyFilesAlreadyExistsInDirectory(Json2SqlTransformOptions transformOptions)
        {
            var directoryForSqlFiles = GenerateSqlDirectoryPath(transformOptions);
            _fileWriter.ThrowIfAnyFilesAlreadyExistsInDirectory(directoryForSqlFiles);
        }

        private Task<string> ReadJsonFile(Json2SqlTransformOptions transformOptions)
        {
            return _fileReader.ReadAllTextAsync(transformOptions.SourceJsonFilePath);
        }

        private void InitSqlBuilder(Json2SqlTransformOptions transformOptions)
        {
            _sqlBuilder.SetSchema(transformOptions.TableSchema);
            _sqlBuilder.SetTableName(transformOptions.TableName);
        }

        private Task CreateFileWithCreateTableSqlStatement(Json2SqlTransformOptions transformOptions)
        {
            var sqlTablePath = $"{transformOptions.TableSchema}_{transformOptions.TableName}";
            var targetSqlFileName = $"001-create-table-{sqlTablePath}.sql";
            var targetSqlFilePath = Path.Combine(GenerateSqlDirectoryPath(transformOptions), targetSqlFileName);

            var createTableStatement = _sqlBuilder.BuildCreateTable();
            return _fileWriter.WriteAllTextAsync(targetSqlFilePath, createTableStatement);
        }

        private string GenerateSqlDirectoryPath(Json2SqlTransformOptions transformOptions)
        {
            return $@"{transformOptions.SourceDirectoryPath}\{transformOptions.SourceJsonFileName}";
        }

        private Task CreateFileWithInsertSqlStatements(Json2SqlTransformOptions transformOptions, string jsonContent)
        {
            var tasks = new List<Task>();

            var rgx1JsonItem = new Regex(@"\}(?=(,)?(\r\n|\n))");
            var jsonItemsMatches = rgx1JsonItem.Matches(jsonContent);
            var jsonItemsCount = jsonItemsMatches.Count;

            var maxLinesPer1InsertValuesSqlFile = transformOptions.MaxLinesPer1InsertValuesSqlFile;
            int filesForCreatingCount = CalculateNewFilesCount(jsonItemsCount, maxLinesPer1InsertValuesSqlFile);

            var fileNumberOffset = 2;
            var sqlTablePath = $"{transformOptions.TableSchema}_{transformOptions.TableName}";

            for (int i = 0; i < filesForCreatingCount; i++)
            {
                var targetSqlFileNameNumberPrefix = $"{(i + fileNumberOffset):000}";
                var targetSqlFileNameNumberSuffix = filesForCreatingCount > 1 ? $"-{(i + 1):000}" : "";
                var targetSqlFileName =
                    $"{targetSqlFileNameNumberPrefix}-insert-values-into-{sqlTablePath}{targetSqlFileNameNumberSuffix}.sql";
                var targetSqlFilePath = Path.Combine(GenerateSqlDirectoryPath(transformOptions), targetSqlFileName);

                var skip = i * maxLinesPer1InsertValuesSqlFile;
                var insertStatement = _sqlBuilder
                    .BuildInsert(jsonContent, skip: skip, limit: maxLinesPer1InsertValuesSqlFile);
                tasks.Add(_fileWriter.WriteAllTextAsync(targetSqlFilePath, insertStatement));
            }

            return Task.WhenAll(tasks);
        }

        private int CalculateNewFilesCount(int jsonItemsCount, int? maxLinesPer1InsertValuesSqlFile)
        {
            if (maxLinesPer1InsertValuesSqlFile == null)
                return 1;

            if (jsonItemsCount % maxLinesPer1InsertValuesSqlFile.Value == 0)
                return jsonItemsCount / maxLinesPer1InsertValuesSqlFile.Value;

            return jsonItemsCount / maxLinesPer1InsertValuesSqlFile.Value + 1;
        }
    }
}