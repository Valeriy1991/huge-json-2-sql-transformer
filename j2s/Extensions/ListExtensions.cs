using System.Collections.Generic;
using System.Text;

namespace j2s.Extensions
{
    public static class ListExtensions
    {
        public static string AsJsonString(this IList<string> items)
        {
            if(items == null)
                return string.Empty;

            var itemsCount = items.Count;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[\n");
            for (int i = 0; i < itemsCount; i++)
            {
                stringBuilder.Append(items[i]);
                if (i < (itemsCount - 1))
                    stringBuilder.Append(",\n");
            }

            stringBuilder.Append("\n]");
            return stringBuilder.ToString();
        }
    }
}