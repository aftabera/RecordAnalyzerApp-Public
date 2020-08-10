using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Controls;
using RecordAnalyzerApp.Model;
using System;

namespace RecordAnalyzerApp.Reporting
{
    public static class SimpleReportCriteria
    {
        static SimpleReportCriteria()
        {
            DatePairCriteria = DatePairOption.Today;
            ShowData = true;
            ShowSummary = true;
            DatePairHandler = new DatePairHandler();
        }

        public static string Title { get; set; }

        public static DatePairOption DatePairCriteria { get; set; }
          
        public static int SortByColumnId { get; set; }

        public static bool Descending { get; set; }

        public static bool ShowCriteria { get; set; }

        public static bool ShowRecordDate { get; set; }

        public static bool ShowRecordTime { get; set; }

        public static bool ShowData { get; set; }

        public static bool ShowSummary { get; set; }

        public static DatePairHandler DatePairHandler { get; private set; }

        public static RecordStore MyRecordStore { get; set; }

        internal static string GetSubTitle()
        {
            return DatePairHandler.ToString();
        }

        internal static bool ShowRecordDateOrTime()
        {
            return ShowRecordDate || ShowRecordTime;
        }

        public static bool ShowCurrentDate { get; set; }

        public static bool ShowCurrentTime { get; set; }

        public static int FilterByColumnId1 { get; set; }

        public static int FilterComparisonId1 { get; set; }

        public static string FilterByText1 { get; set; }

        public static int FilterByColumnId2 { get; set; }

        public static int FilterComparisonId2 { get; set; }

        public static string FilterByText2 { get; set; }

        internal static string GetFilterComparisonString(FilterOptions comparisonType, bool compareAsNumber, string value)
        {
            switch (comparisonType)
            {
                case FilterOptions.EqualTo:

                    return string.Format("= {0}", 
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.NotEqualTo:

                    return string.Format("<> {0}",
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.LessThan:

                    return string.Format("< {0}",
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.LessThanAndEqualTo:

                    return string.Format("<= {0}",
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.GreatorThan:

                    return string.Format("> {0}",
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.GreatorThanAndEqualTo:

                    return string.Format(">= {0}",
                        compareAsNumber ? value : Functions.GetFullQuoted(value)
                        );

                case FilterOptions.Contains:

                    return string.Format("LIKE '%{0}%'", Functions.GetEscapedLikeValue(value));

                case FilterOptions.NotContains:

                    return string.Format("NOT LIKE '%{0}%'", Functions.GetEscapedLikeValue(value));

                case FilterOptions.BeginsWith:

                    return string.Format("LIKE '{0}%'", Functions.GetEscapedLikeValue(value));

                case FilterOptions.EndsWith:

                    return string.Format("LIKE '%{0}'", Functions.GetEscapedLikeValue(value));

                default:
                    return string.Empty;
            }
        }

        internal static bool Is1stDataFilterRequired()
        {
            return !string.IsNullOrWhiteSpace(FilterByText1);
        }

        internal static bool Is2ndDataFilterRequired()
        {
            return !string.IsNullOrWhiteSpace(FilterByText2);
        }
    }
}
