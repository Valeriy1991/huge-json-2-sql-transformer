using System.IO;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Json.Abstract;

namespace HugeJson2SqlTransformer.Json
{
    public class MongoDbCompass_1_17_0_JsonFileReader : IJsonFileReader
    {
        public async Task<string> ReadAllTextAsync(string jsonFilePath)
        {
            var mongoDbCompassJsonLines = await File.ReadAllLinesAsync(jsonFilePath);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[\n");
            var linesCount = mongoDbCompassJsonLines.Length;
            for (int i = 0; i < linesCount; i++)
            {
                stringBuilder.Append(mongoDbCompassJsonLines[i]);
                if (i < linesCount - 1)
                {
                    stringBuilder.Append(",\n");
                }
            }

            stringBuilder.Append("\n]");
            return stringBuilder.ToString();
        }
    }
}