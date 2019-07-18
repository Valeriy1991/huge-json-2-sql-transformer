namespace HugeJson2SqlTransformer.Transformers
{
    public class Json2SqlTransformOptions
    {
        public string SourceJsonFile { get; set; }
        public string TargetSqlFile { get; set; }
        public bool NeedSplitAllSqlDeclarations { get; set; } = true;
        public int? InsertSplitLines { get; set; }

        public string TableSchema { get; set; }
        public string TableName { get; set; }
    }
}