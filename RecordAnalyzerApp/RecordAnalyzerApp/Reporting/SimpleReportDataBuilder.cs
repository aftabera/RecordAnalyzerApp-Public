using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace RecordAnalyzerApp.Reporting
{
    public class SimpleReportDataBuilder
    {
        List<RecordStoreDataType> _dataTypes;

        public SimpleReportDataBuilder()
        {
            MyDataView = new DataView();
        }

        public DataView MyDataView { get; private set; }

        public List<SimpleReportSummary> MainSummary { get; private set; }

        /// <summary>
        /// Start building report data from report criteria for report viewer to display
        /// </summary>
        public async Task BuildReportData()
        {
            if (SimpleReportCriteria.ShowData || SimpleReportCriteria.ShowSummary)
            {
                await GetDataTypes();
                await BuildRowsDataTable();
            }

            if (SimpleReportCriteria.ShowData)
            {
                await FilterRowsData();
                await SortRowsData();
            }

            if (SimpleReportCriteria.ShowSummary)
            {
                await BuildSummary();
            }
        }

        private Task FilterRowsData()
        {
            string rowFilter = string.Empty;

            if (SimpleReportCriteria.Is1stDataFilterRequired())
            {
                rowFilter = GetRowFilter(SimpleReportCriteria.FilterByColumnId1,
                    (Controls.FilterOptions)SimpleReportCriteria.FilterComparisonId1,
                    SimpleReportCriteria.FilterByText1
                    );
            }

            if (SimpleReportCriteria.Is2ndDataFilterRequired())
            {
                string myFilter = GetRowFilter(SimpleReportCriteria.FilterByColumnId2,
                    (Controls.FilterOptions)SimpleReportCriteria.FilterComparisonId2,
                    SimpleReportCriteria.FilterByText2
                    );

                rowFilter = string.IsNullOrWhiteSpace(rowFilter) ? myFilter : string.Format("{0} AND {1}", rowFilter, myFilter);
            }

            if (!string.IsNullOrWhiteSpace(rowFilter))
            {
                try
                {
                    MyDataView.RowFilter = rowFilter;
                }
                catch { }
            }

            return Task.CompletedTask;
        }

        private string GetRowFilter(int columnId, Controls.FilterOptions comparisonType, string value)
        {
            var type = _dataTypes.Find(p => p.Id == columnId);
            if (type != null)
            {
                string myFilter = SimpleReportCriteria.GetFilterComparisonString(comparisonType, type.IsNumericDataType(), value);
                return string.Format("[{0}] {1}", Common.Functions.GetEscapedName(type.Title), myFilter);
            }
            return string.Empty;
        }

        private Task SortRowsData()
        {
            if (SimpleReportCriteria.SortByColumnId > 0)
            {
                var type = _dataTypes.Find(p => p.Id == SimpleReportCriteria.SortByColumnId);
                if (type != null)
                {
                    try
                    {
                        MyDataView.Sort = string.Format("[{0}] {1}",
                            Common.Functions.GetEscapedName(type.Title),
                            SimpleReportCriteria.Descending ? "DESC" : "ASC"
                            );
                    }
                    catch { }
                }
            }
            return Task.CompletedTask;
        }

        private Task BuildSummary()
        {
            var summary = new SimpleReportSummaryBuilder();

            // Input data to summary builder
            foreach (DataRowView row in MyDataView)
            {
                for (int i = 0; i < _dataTypes.Count; i++)
                {
                    var type = _dataTypes[i];
                    if (type.IsNumericDataType())
                    {
                        summary.AddInput(type.Title, row[i].ToString());
                    }
                }
            }
            
            // Build & get summary
            MainSummary = summary.BuildSummary();

            return Task.CompletedTask;
        }

        private async Task BuildRowsDataTable()
        {
            var table = new DataTable("MyTable");

            foreach (var item in _dataTypes)
            {
                table.Columns.Add(item.Title, item.GetDataType());
            }

            // add column for datetime data type  
            string dtColumnName = string.Format("DTCN{0}", DateTime.Now.Ticks);
            if (SimpleReportCriteria.ShowRecordDateOrTime())
            {
                table.Columns.Add(dtColumnName, typeof(DateTime));
            }

            var allDetails = await MyDB.C.Table<RecordStoreDataDetail>()
                .Where(p => p.RecordStoreId == SimpleReportCriteria.MyRecordStore.Id)
                .OrderBy(p => p.RecordStoreDataMasterId)
                .OrderBy(p => p.RecordStoreDataTypeId)
                .ToListAsync();

            var allMasters = await MyDB.C.Table<RecordStoreDataMaster>()
                .Where(p => p.RecordStoreId == SimpleReportCriteria.MyRecordStore.Id &&
                p.MasterDateTime >= SimpleReportCriteria.DatePairHandler.FromNow &&
                p.MasterDateTime <= SimpleReportCriteria.DatePairHandler.ToThen)
                .ToListAsync();

            allDetails = allDetails.Where(d => allMasters.Any(m => m.Id.Equals(d.RecordStoreDataMasterId))).ToList();

            if (allDetails.Count == 0)
            {
                MyDataView.Table = table;
                return;
            }   

            int temp = allDetails[0].RecordStoreDataMasterId;

            var row = table.NewRow();
            if (SimpleReportCriteria.ShowRecordDateOrTime())
            {
                row[dtColumnName] = allMasters.Find(p => p.Id == temp).MasterDateTime;
            }

            int index = 0;
            foreach (var item in allDetails)
            {
                if (temp != item.RecordStoreDataMasterId)
                {
                    temp = item.RecordStoreDataMasterId;
                    table.Rows.Add(row);

                    row = table.NewRow();
                    if (SimpleReportCriteria.ShowRecordDateOrTime())
                    {
                        row[dtColumnName] = allMasters.Find(p => p.Id == temp).MasterDateTime;
                    }

                    index = 0;
                }

                var type = _dataTypes[index];

                try
                {
                    if (type.IsListDataType())
                    {
                        int.TryParse(item.Value, out int i);
                        row[index] = await type.GetListElementTitle(i);
                    }
                    else
                    {
                        row[index] = Convert.ChangeType(item.Value, table.Columns[index].DataType);
                    }                    
                }
                catch 
                {
                    row[index] = type.GetDefaultData();
                }
                                
                index++;
            }
            table.Rows.Add(row);

            allDetails = null;

            MyDataView.Table = table;
        }

        private async Task GetDataTypes()
        {
            _dataTypes = await SimpleReportCriteria.MyRecordStore.GetAllFields();
        }
    }
}
