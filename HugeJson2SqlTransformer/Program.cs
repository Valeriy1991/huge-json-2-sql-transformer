using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Files.Abstract;
using HugeJson2SqlTransformer.Files.Readers.Json;
using HugeJson2SqlTransformer.Files.Writers;
using HugeJson2SqlTransformer.Sql;
using HugeJson2SqlTransformer.Sql.Builders;
using HugeJson2SqlTransformer.Sql.TableDefinition;
using HugeJson2SqlTransformer.Transformers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace HugeJson2SqlTransformer
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("j2s - JSON to SQL files transformer.");

            var jsonSourceFilePath = GetArgumentValue("--source-json", args);
            var tableName = GetArgumentValue("--table-name", args);
            var schema = GetArgumentValue("--schema", args);
            var limitInserts = Convert.ToInt32(GetArgumentValue("--limit-inserts", args));
            var columnsDefinitionFilePath = GetArgumentValue("--columns-definition-file", args);
            var tableColumns = ParseTableDeColumnsDefinition(columnsDefinitionFilePath);

            var jsonFileReader = new MongoDbCompass_1_17_0_JsonFileReader();
            var fileWriter = new FileWriter();
            var sqlBuilder = new PostgreSqlBuilder(tableColumns);

            var transformer =
                new Json2SqlTransformer(jsonFileReader, fileWriter, sqlBuilder);

            var transformOptions = new Json2SqlTransformOptions()
            {
                SourceJsonFilePath = jsonSourceFilePath,
                TableName = tableName,
                TableSchema = schema,
                MaxLinesPer1InsertValuesSqlFile = limitInserts
            };
            
            var transformResult = await transformer.ExecuteAsync(transformOptions);
            if (transformResult.Success)
            {
                Console.WriteLine("Transform success!");
            }
            else
            {
                Console.WriteLine($"Transformation was failed with ERROR:\n\t{transformResult.ToString()}");
            }
            Console.WriteLine("\nPress any key for exit...");
            Console.ReadLine();
        }

        private static string GetArgumentValue(string argumentName, string[] args)
        {
            var argument = args.FirstOrDefault(e => e.StartsWith(argumentName));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            var keyValueSeparator = '=';
            var keyValueSeparatorPosition = argument.IndexOf(keyValueSeparator);
            return argument.Substring(keyValueSeparatorPosition + 1);
        }

        private static List<TableColumn> ParseTableDeColumnsDefinition(string file)
        {
            var jsonContent = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<List<TableColumn>>(jsonContent);
        }
    }
}
