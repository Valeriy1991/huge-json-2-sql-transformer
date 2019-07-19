namespace HugeJson2SqlTransformer.Extensions
{
    public static class StringExtensions
    {

        public static string FixSingleQuotes(this string inputString)
        {
            return inputString?.Replace("'", "''");
        }
    }
}