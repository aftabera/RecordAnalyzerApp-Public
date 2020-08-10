using RecordAnalyzerApp.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Model
{
    [Table("RecordStores")]
    public class RecordStore
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string StoreName { get; set; }

        internal static async Task<int> Delete(RecordStore store)
        {
            int count = 0;

            // delete detail table data
            count += await RecordStoreDataDetail.Delete(store.Id);

            // delete master table data
            count += await RecordStoreDataMaster.Delete(store.Id);

            // delete data types defined for the store
            count += await RecordStoreDataType.Delete(store.Id);

            // delete orphan lists
            count += await RecordStoreDataTypeList.Delete();

            // finally delete store
            count += await MyDB.C.DeleteAsync(store);

            return count;
        }

        internal async Task<List<RecordStoreDataType>> GetAllFields()
        {
            return await MyDB.C.Table<RecordStoreDataType>()
                .Where(p => p.RecordStoreId == Id)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        internal async Task<bool> HasData()
        {
            return await RecordStoreDataDetail.HasData(Id);
        }
    }
}
