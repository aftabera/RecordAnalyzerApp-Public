using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Controls;
using RecordAnalyzerApp.Model;

namespace RecordAnalyzerApp.Graph
{
    public static class SimpleGraphCriteria
    {
        static SimpleGraphCriteria()
        {
            DatePairCriteria = DatePairOption.Today;
            DateGroupingCriteria = DateGroupingOption.None;
            ChartType = ChartType.BarChart;

            DatePairHandler = new DatePairHandler();
        }

        public static DatePairOption DatePairCriteria { get; set; }

        public static DateGroupingOption DateGroupingCriteria { get; set; }
        
        public static int GroupByColumnId { get; set; }  
        
        public static int GraphValueFromColumnId { get; set; }

        public static DatePairHandler DatePairHandler { get; private set; }

        public static RecordStore MyRecordStore { get; set; }

        public static ChartType ChartType { get; set; }
    }
}
