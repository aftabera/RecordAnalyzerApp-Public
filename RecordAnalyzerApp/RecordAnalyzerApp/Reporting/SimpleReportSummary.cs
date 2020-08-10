using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RecordAnalyzerApp.Reporting
{
    public class SimpleReportSummary
    {
        public string Title { get; set; }
        public int Count { get; set; }
        public decimal Sum { get; set; }
        public decimal Avg { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0}: Sum: {1:0.00}, Avg: {2:0.00}, Max: {3:0.00}, Min: {4:0.00}",
                Title, Sum, Avg, Max, Min
                );
        }
    }
}
