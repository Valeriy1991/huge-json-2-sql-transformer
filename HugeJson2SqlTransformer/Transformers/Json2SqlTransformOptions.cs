using System.IO;

namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformOptions
    {
        /// <summary>
        /// Full path to JSON file
        /// </summary>
        public string SourceJsonFilePath { get; set; }

        internal string SourceJsonFileName => Path.GetFileNameWithoutExtension(SourceJsonFilePath);

        public bool NeedSplitAllSqlDeclarations { get; set; } = true;
        public int? InsertSplitLines { get; set; }

        public string TableSchema { get; set; }
        public string TableName { get; set; }
    }
}