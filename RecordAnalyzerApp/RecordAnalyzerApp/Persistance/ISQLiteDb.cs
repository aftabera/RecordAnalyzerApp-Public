using SQLite;

namespace RecordAnalyzerApp.Persistance
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
