using RecordAnalyzerApp.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Model
{
    [Table("RecordStoreDataTypes")]
    public class RecordStoreDataType
    {
        public static readonly string YES = "Yes";
        public static readonly string NO = "No";

        List<RecordStoreDataTypeList> _myList = null;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public int DataTypeId { get; set; } 

        public int RecordStoreId { get; set; }

        public string DataTypeName 
        { 
            get
            {
                return ((DataTypes)DataTypeId).ToString();
            }
        }

        internal static Task<int> Delete(int storeId)
        {
            return MyDB.C.Table<RecordStoreDataType>().DeleteAsync(p => p.RecordStoreId == storeId);
        }

        internal bool IsNumericDataType()
        {
            return DataTypeId == (int)DataTypes.Numeric;
        }

        internal Type GetDataType()
        {
            DataTypes type = (DataTypes)DataTypeId;
            switch (type)
            {
                case DataTypes.Numeric:
                    return typeof(decimal);

                case DataTypes.Date:
                case DataTypes.Time:
                    return typeof(DateTime);

                case DataTypes.Text:
                case DataTypes.YesNo:
                case DataTypes.Email:
                case DataTypes.Telephone:
                case DataTypes.Url:
                case DataTypes.MultilineText:
                case DataTypes.List:
                case DataTypes.Password:
                default:
                    return typeof(string);
            }
        }

        internal bool IsGroupableDataType()
        {
            DataTypes type = (DataTypes)DataTypeId;
            switch (type)
            {
                case DataTypes.Text:
                case DataTypes.Numeric:
                case DataTypes.List:
                case DataTypes.YesNo:
                    return true;
                default:
                    return false;
            }
        }

        internal bool IsListDataType()
        {
            return DataTypeId == (int)DataTypes.List;
        }

        internal async Task<string> GetListElementTitle(int index)
        {
            if (_myList == null)
            {
                _myList = await RecordStoreDataTypeList.GetByRecordStoreDataTypeId(Id);
            }

            if (index < _myList.Count)
            {
                return _myList[index].ListElementTitle;
            }

            return string.Empty;
        }

        internal object GetDefaultData()
        {
            DataTypes type = (DataTypes)DataTypeId;
            switch (type)
            {
                case DataTypes.Numeric:

                    return 0;

                case DataTypes.Date:
                case DataTypes.Time:

                    return DateTime.Now;

                case DataTypes.YesNo:

                    return "No";
                
                default:

                    return string.Empty;
            }
        }
    }
}
