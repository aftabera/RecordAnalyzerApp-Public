using System.Text;

namespace RecordAnalyzerApp.Common
{
    public static class Functions
    {
        public static string GetEscapedName(string value)
        {
            return value.Replace(@"\", @"\\").Replace("]", @"\]");
        }

        public static string GetProperQuoted(string value)
        {
            return value.Replace("'", "''");
        }

        public static string GetQuoteEncoded(string value)
        {
            return string.Format("'{0}'", value);
        }

        public static string GetFullQuoted(string value)
        {
            return GetQuoteEncoded(GetProperQuoted(value));
        }

        public static string GetEscapedLikeValue(string valueWithoutWildcards)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
