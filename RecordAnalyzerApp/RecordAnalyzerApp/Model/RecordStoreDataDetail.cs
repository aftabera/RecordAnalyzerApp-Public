using RecordAnalyzerApp.Persistance;
using SQLite;
using System;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Model
{
    public class RecordStoreDataDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Value { get; set; }

        public int RecordStoreDataTypeId { get; set; }

        public int RecordStoreDataMasterId { get; set; }

        public int RecordStoreId { get; set; }

        internal static Task<int> Delete(int storeId)
        {
            return MyDB.C.Table<RecordStoreDataDetail>().DeleteAsync(p => p.RecordStoreId == storeId);
        }

        internal static async Task<bool> HasData(int storeId)
        {
            return await MyDB.C.Table<RecordStoreDataDetail>().Where(p => p.RecordStoreId == storeId).CountAsync() > 0;
        }
    }
}
