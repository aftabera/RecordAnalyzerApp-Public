using RecordAnalyzerApp.Persistance;
using SQLite;
using System;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Model
{
    public class RecordStoreDataMaster
    {
        public RecordStoreDataMaster()
        {
            MasterDateTime = DateTime.Now;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime MasterDateTime { get; set; }

        public string PrimaryValue { get; set; }

        public int RecordStoreId { get; set; }

        internal static Task<int> Delete(int storeId)
        {
            return MyDB.C.Table<RecordStoreDataMaster>().DeleteAsync(p => p.RecordStoreId == storeId);
        }

        //internal string GetFirstColumnValue()
        //{
        //    string value = string.Empty;

        //    try
        //    {
        //        var topDetail = MyDB.C.Table<RecordStoreDataDetail>().Where(m => m.RecordStoreDataMasterId == Id).Take(1).ToListAsync();
        //        if (topDetail.Result.Count == 0)
        //            value = topDetail.Result[0].Value;
        //    }
        //    catch { }

        //    return value;
        //}

        //public override string ToString()
        //{
        //    return string.Format("{0},{1},{2}", Id, CreatedOn, ModifiedOn);
        //}
    }
}
