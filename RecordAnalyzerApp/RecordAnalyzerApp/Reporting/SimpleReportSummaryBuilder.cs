using System;
using System.Collections.Generic;

namespace RecordAnalyzerApp.Reporting
{
    public class SimpleReportSummaryBuilder
    {
        const int DEFAULT_ROUNDING = 2;

        Dictionary<string, List<string>> _dict = new Dictionary<string, List<string>>();

        internal void AddInput(string title, string value)
        {
            if (_dict.ContainsKey(title))
            {
                _dict[title].Add(value);
            }                
            else
            {
                var list = new List<string>();
                list.Add(value);
                _dict.Add(title, list);
            }
        }

        internal List<SimpleReportSummary> BuildSummary()
        {
            var list = new List<SimpleReportSummary>();
            foreach (var item in _dict)
            {
                list.Add(GetSummary(item.Key, item.Value));
            }
            list.TrimExcess();
            return list;
        }

        private SimpleReportSummary GetSummary(string title, List<string> value)
        {
            int count = 0;
            decimal sum = 0, avg = 0, max = 0, min = 0;

            foreach (var item in value)
            {
                decimal temp;
                if (decimal.TryParse(item, out temp))
                {
                    count++;
                    sum += temp;
                    
                    if (count == 1)
                    {
                        max = min = temp;
                    }
                    else
                    {
                        max = temp > max ? temp : max;
                        min = temp < min ? temp : min;
                    }
                }
            }

            if (count > 0)
            {
                avg = sum / count;
            }

            return new SimpleReportSummary 
            {
                Title = title,
                Count = count,
                Sum = Math.Round(sum, DEFAULT_ROUNDING),
                Avg = Math.Round(avg, DEFAULT_ROUNDING),
                Max = Math.Round(max, DEFAULT_ROUNDING),
                Min = Math.Round(min, DEFAULT_ROUNDING)
            };
        }
    }
}