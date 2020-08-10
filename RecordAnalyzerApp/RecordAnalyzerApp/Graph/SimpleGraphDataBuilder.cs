using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Graph
{
    public class SimpleGraphDataBuilder
    {
        List<RecordStoreDataType> _dataTypes;
        
        /// <summary>
        /// Start building report data from report criteria for report viewer to display
        /// </summary>
        public async Task<List<GraphDataItem>> BuildReportData()
        {
            await GetDataTypes();
            return await BuildGraphData();
        }

        private async Task<List<GraphDataItem>> BuildGraphData()
        {
            var table = new DataTable("MyTable");
            
            foreach (var item in _dataTypes)
            {
                table.Columns.Add(item.Title, item.GetDataType());
            }

            // add column for datetime data type
            string dtColumnName = string.Format("DTCN{0}", DateTime.Now.Ticks);
            table.Columns.Add(dtColumnName, typeof(DateTime));

            var allDetails = await MyDB.C.Table<RecordStoreDataDetail>()
                .Where(p => p.RecordStoreId == SimpleGraphCriteria.MyRecordStore.Id)
                .OrderBy(p => p.RecordStoreDataMasterId)
                .OrderBy(p => p.RecordStoreDataTypeId)
                .ToListAsync();

            var allMasters = await MyDB.C.Table<RecordStoreDataMaster>()
                .Where(p => p.RecordStoreId == SimpleGraphCriteria.MyRecordStore.Id &&
                p.MasterDateTime >= SimpleGraphCriteria.DatePairHandler.FromNow &&
                p.MasterDateTime <= SimpleGraphCriteria.DatePairHandler.ToThen)
                .ToListAsync();

            allDetails = allDetails.Where(d => allMasters.Any(m => m.Id.Equals(d.RecordStoreDataMasterId))).ToList();

            if (allDetails.Count == 0)
            {
                return null;
            }

            int temp = allDetails[0].RecordStoreDataMasterId;

            var row = table.NewRow();
            row[dtColumnName] = allMasters.Find(p => p.Id == temp).MasterDateTime;

            int index = 0;
            foreach (var item in allDetails)
            {
                if (temp != item.RecordStoreDataMasterId)
                {
                    temp = item.RecordStoreDataMasterId;

                    table.Rows.Add(row);

                    row = table.NewRow();
                    row[dtColumnName] = allMasters.Find(p => p.Id == temp).MasterDateTime;

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
            allMasters = null;

            return await BuildGraphData(table, dtColumnName);
        }

        private async Task<List<GraphDataItem>> BuildGraphData(DataTable table, string dateColumnName)
        {
            // Gather required data
            List<DatePairHandler> datePairList = SimpleGraphCriteria.DatePairHandler.GetDatePairList(SimpleGraphCriteria.DateGroupingCriteria);

            string groupByColumnName = GetGroupByColumnName(SimpleGraphCriteria.GroupByColumnId);

            List<string> listOfValues = null;
            if (!string.IsNullOrWhiteSpace(groupByColumnName))
            {
                listOfValues = await GetListOfValuesForGrouping(table, SimpleGraphCriteria.GroupByColumnId);
            }

            string valueByColumnName = GetValueByColumnName(SimpleGraphCriteria.GraphValueFromColumnId);
            if (string.IsNullOrWhiteSpace(valueByColumnName))
            {
                return null;
            }

            bool sumValueColumn = IsSumValueColumn(SimpleGraphCriteria.GraphValueFromColumnId);

            // Finally build graph data
            return BuildGraphData(table, datePairList, dateColumnName, groupByColumnName, listOfValues, valueByColumnName, sumValueColumn);
        }

        private List<GraphDataItem> BuildGraphData(
            DataTable table, 
            List<DatePairHandler> datePairList, 
            string dateColumnName, 
            string groupByColumnName,
            List<string> listOfValues,
            string valueByColumnName, 
            bool sumValueColumn
            )
        {
            List<GraphDataItem> graphData = new List<GraphDataItem>();

            foreach (var dp in datePairList)
            {
                string expression = string.Format("{0}([{1}])", 
                    sumValueColumn ? "sum" : "count",
                    Functions.GetEscapedName(valueByColumnName)
                    );

                string filter = string.Format("[{0}]>='{1}' and [{0}]<='{2}'", 
                    Functions.GetEscapedName(dateColumnName), 
                    dp.FromDateString, 
                    dp.ToDateString
                    );

                float value;

                if (string.IsNullOrWhiteSpace(groupByColumnName) || listOfValues == null)
                {
                    try
                    {
                        value = (float)Convert.ChangeType(table.Compute(expression, filter), typeof(float));
                    }
                    catch
                    {
                        value = 0;
                    }

                    if (value != 0)
                    {
                        graphData.Add(new GraphDataItem
                        {
                            Title = dp.Title,
                            Value = value
                        });
                    }
                }
                else
                {
                    foreach (var compareValue in listOfValues)
                    {
                        string myFilter = string.Format("{0} and [{1}]='{2}'", 
                            filter, 
                            Functions.GetEscapedName(groupByColumnName), 
                            Functions.GetProperQuoted(compareValue)
                            );

                        try
                        {
                            value = (float)Convert.ChangeType(table.Compute(expression, myFilter), typeof(float));
                        }
                        catch
                        {
                            value = 0;
                        }
                        
                        if (value != 0)
                        {
                            graphData.Add(new GraphDataItem
                            {
                                Title = datePairList.Count == 1 ? compareValue : string.Format("{0}[{1}]", dp.Title, compareValue),
                                Value = value
                            });
                        }
                    }
                }
            }

            graphData.TrimExcess();

            return graphData;
        }

        private async Task<List<string>> GetListOfValuesForGrouping(DataTable table, int groupByColumnId)
        {
            RecordStoreDataType type = _dataTypes.Find(p => p.Id == groupByColumnId);

            List<string> list = new List<string>();

            if (type != null)
            {
                if (type.DataTypeId == (int)DataTypes.Text || type.DataTypeId == (int)DataTypes.Numeric)
                {
                    list = GetListFromTextOrNumericDataTypes(table, type.Title);
                }
                else if (type.DataTypeId == (int)DataTypes.YesNo)
                {
                    list.Add(RecordStoreDataType.YES);
                    list.Add(RecordStoreDataType.NO);
                }
                else if (type.DataTypeId == (int)DataTypes.List)
                {
                    var valuesList = await RecordStoreDataTypeList.GetByRecordStoreDataTypeId(groupByColumnId);
                    foreach (var value in valuesList)
                    {
                        list.Add(value.ListElementTitle);
                    }
                }
            }

            if (list.Count > 0)
            {
                list.TrimExcess();
                return list;
            }

            return null;
        }

        private List<string> GetListFromTextOrNumericDataTypes(DataTable table, string columnName)
        {
            List<string> list = new List<string>();

            var myTable = table.DefaultView.ToTable(true, columnName);

            if (myTable.Rows.Count > 0)
            {
                foreach (DataRow item in myTable.Rows)
                {
                    list.Add(item[0].ToString());
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        private bool IsSumValueColumn(int valueByColumnId)
        {
            RecordStoreDataType type = _dataTypes.Find(p => p.Id == valueByColumnId);
            return type != null && type.IsNumericDataType();
        }

        private string GetValueByColumnName(int graphValueFromColumnId)
        {
            RecordStoreDataType type = _dataTypes.Find(p => p.Id == graphValueFromColumnId);
            return type != null ? type.Title : string.Empty;
        }

        private string GetGroupByColumnName(int groupByColumnId)
        {
            RecordStoreDataType type = _dataTypes.Find(p => p.Id == groupByColumnId);
            return type != null ? type.Title : string.Empty;
        }

        private async Task GetDataTypes()
        {
            _dataTypes = await SimpleGraphCriteria.MyRecordStore.GetAllFields();
        }
    }
}
