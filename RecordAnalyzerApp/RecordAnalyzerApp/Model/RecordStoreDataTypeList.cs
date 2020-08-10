using RecordAnalyzerApp.Persistance;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecordAnalyzerApp.Model
{
    public class RecordStoreDataTypeList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Index { get; set; }


        [MaxLength(50)]
        public string ListElementTitle { get; set; }

        public int RecordStoreDataTypeId { get; set; }

        internal static async Task<List<RecordStoreDataTypeList>> GetByRecordStoreDataTypeId(int id)
        {
            return await MyDB.C.Table<RecordStoreDataTypeList>()
                .Where(p => p.RecordStoreDataTypeId == id)
                .OrderBy(p => p.Index)
                .ToListAsync();
        }

        internal static Task<int> Delete()
        {
            string q;

            q = @"DELETE FROM RecordStoreDataTypeList WHERE RecordStoreDataTypeId NOT IN (
                    SELECT Id FROM RecordStoreDataTypes
                )";

            return MyDB.C.ExecuteAsync(q);
        }
    }
}
