using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Reporting
{
    public class SimpleReportBuilder
    {
        SimpleReportDataBuilder _data;
        StringBuilder _sb;

        public SimpleReportBuilder(SimpleReportDataBuilder data)
        {
            _data = data;
            _sb = new StringBuilder();
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        ~SimpleReportBuilder()
        {
            _sb.Clear();
        }

        public Task BuildReport()
        {
            _sb.Append(@"<html>");
            _sb.Append(@"<head><style>
table, h2, h3, h4, h5 {
   width: 100%;
}

th, td, h2, h3, h4, h5 {
  text-align: center;
  padding: 5px 0px 5px 0px;
}

tr:nth-child(odd){background-color: #e6e6e6}
tr:nth-child(even){background-color: #f2f2f2}

.main th {
  background-color: #354458;
  color: white;
}

.summary th {
  color: #354458;
  background-color: white;
}

h2, h4 { color: #354458; }

h3, h5 { color: #355658; }

</style></head>");
            _sb.Append(@"<body>");

            if (!string.IsNullOrWhiteSpace(SimpleReportCriteria.Title))
            {
                _sb.AppendFormat(@"<h2>{0}</h2>", SimpleReportCriteria.Title);
            }

            if (SimpleReportCriteria.ShowCriteria)
            {
                _sb.AppendFormat(@"<h3>{0}</h3>", SimpleReportCriteria.GetSubTitle());
            }

            if (SimpleReportCriteria.ShowCurrentDate)
            {
                _sb.AppendFormat(@"<h4>{0:dd MMM yyyy}</h4>", DateTime.Now);
            }

            if (SimpleReportCriteria.ShowData)
            {
                BeginTableTag("main");
                ColumnHeadings();
                RowsData();
                EndTableTag();
            }

            if (SimpleReportCriteria.ShowSummary)
            {
                AddLineBreak();
                ShowMainSummary();
            }

            if (SimpleReportCriteria.ShowCurrentTime)
            {
                _sb.AppendFormat(@"<h5>{0:dd MMM yyyy HH:mm:ss}</h5>", DateTime.Now);
            }
            _sb.Append(@"</body>");
            _sb.Append(@"</html>");

            return Task.CompletedTask;
        }

        private void ShowMainSummary()
        {
            BeginTableTag("summary");

            BeginTableRow();

            BeginTableHeader();
            _sb.Append("Summary");
            EndTableHeader();

            /*
            BeginTableHeader();
            _sb.Append("#");
            EndTableHeader();
            //*/

            BeginTableHeader();
            _sb.Append("Sum");
            EndTableHeader();

            BeginTableHeader();
            _sb.Append("Avg");
            EndTableHeader();

            BeginTableHeader();
            _sb.Append("Max");
            EndTableHeader();

            BeginTableHeader();
            _sb.Append("Min");
            EndTableHeader();

            EndTableRow();

            foreach (var item in _data.MainSummary)
            {
                BeginTableRow();

                BeginTableData();
                _sb.Append(item.Title);
                EndTableData();

                /*
                BeginTableData();
                _sb.Append(item.Count);
                EndTableData();
                //*/

                BeginTableData();
                _sb.Append(item.Sum);
                EndTableData();

                BeginTableData();
                _sb.Append(item.Avg);
                EndTableData();

                BeginTableData();
                _sb.Append(item.Max);
                EndTableData();

                BeginTableData();
                _sb.Append(item.Min);
                EndTableData();

                EndTableRow();
            }

            EndTableTag();
        }

        private void RowsData()
        {
            foreach (DataRowView row in _data.MyDataView)
            {
                BeginTableRow();

                int count;

                if (SimpleReportCriteria.ShowRecordDateOrTime())
                {
                    count = row.Row.ItemArray.Length - 1;

                    if (SimpleReportCriteria.ShowRecordDate)
                    {
                        BeginTableData();
                        _sb.AppendFormat("{0:dd MMM yyyy}", Convert.ToDateTime(row.Row[count].ToString()));
                        EndTableData();
                    }

                    if (SimpleReportCriteria.ShowRecordTime)
                    {
                        BeginTableData();
                        _sb.AppendFormat("{0:HH:mm:ss}", Convert.ToDateTime(row.Row[count].ToString()));
                        EndTableData();
                    }
                }
                else
                {
                    count = row.Row.ItemArray.Length;
                }

                for (int i = 0; i < count; i++)
                {
                    BeginTableData();
                    _sb.Append(row.Row[i].ToString());
                    EndTableData();
                }

                EndTableRow();
            }
        }

        private void ColumnHeadings()
        {
            BeginTableRow();

            int count;

            if (SimpleReportCriteria.ShowRecordDateOrTime())
            {
                count = _data.MyDataView.Table.Columns.Count - 1;

                if (SimpleReportCriteria.ShowRecordDate)
                {
                    BeginTableHeader();
                    _sb.Append("Date");
                    EndTableHeader();
                }

                if (SimpleReportCriteria.ShowRecordTime)
                {
                    BeginTableHeader();
                    _sb.Append("Time");
                    EndTableHeader();
                }
            }
            else
            {
                count = _data.MyDataView.Table.Columns.Count;
            }

            for (int i = 0; i < count; i++)
            {
                BeginTableHeader();
                _sb.Append(_data.MyDataView.Table.Columns[i].ColumnName);
                EndTableHeader();
            }

            EndTableRow();
        }

        private void AddLineBreak()
        {
            _sb.Append(@"<br/>");
        }

        private void BeginTableRow()
        {
            _sb.Append(@"<tr>");
        }

        private void EndTableRow()
        {
            _sb.Append(@"</tr>");
        }

        private void BeginTableHeader()
        {
            _sb.Append(@"<th>");
        }

        private void EndTableHeader()
        {
            _sb.Append(@"</th>");
        }

        private void BeginTableData()
        {
            _sb.Append(@"<td>");
        }

        private void EndTableData()
        {
            _sb.Append(@"</td>");
        }

        private void BeginTableTag(string clsName)
        {
            _sb.Append(string.Format(@"<table class='{0}'>", clsName));
        }

        private void EndTableTag()
        {
            _sb.Append(@"</table>");
        }
    }
}
