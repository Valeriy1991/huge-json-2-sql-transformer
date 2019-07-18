using System.IO;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformOptions
    {
        /// <summary>
        /// Full path to JSON file
        /// </summary>
        public string SourceJsonFilePath { get; set; }

        internal string SourceDirectoryPath => Path.GetDirectoryName(SourceJsonFilePath);
        internal string SourceJsonFileName => Path.GetFileNameWithoutExtension(SourceJsonFilePath);

        public int? MaxLinesPer1InsertValuesSqlFile { get; set; }

        public string TableSchema { get; set; }
        public string TableName { get; set; }
    }
}