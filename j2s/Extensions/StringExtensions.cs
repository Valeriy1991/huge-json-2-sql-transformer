namespace j2s.Extensions
{
    public static class StringExtensions
    {

        public static string FixSingleQuotes(this string inputString)
        {
            return inputString?.Replace("'", "''");
        }
    }
}