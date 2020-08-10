using RecordAnalyzerApp.Model;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RecordAnalyzerApp.Persistance
{
    public class MyDB : ISQLiteDb
    {
        const string _DB_NAME = "_record_analyzer_db.db3";
        const string _DB_BACKUP_NAME = "My_DB_Backup.db3";

        public SQLiteAsyncConnection GetConnection()
        {
            return GetConnection(GetDBFilePath());
        }

        static string GetDBFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, _DB_NAME);
        }

        public SQLiteAsyncConnection GetConnection(string path)
        {
            return new SQLiteAsyncConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
        }

        public static SQLiteAsyncConnection C { get; set; }

        public async static Task InitializeDatabase()
        {
            if (C == null) C = new MyDB().GetConnection();

            await C.CreateTableAsync<RecordStore>();
            await C.CreateTableAsync<RecordStoreDataType>();
            await C.CreateTableAsync<RecordStoreDataMaster>();
            await C.CreateTableAsync<RecordStoreDataDetail>();
            await C.CreateTableAsync<RecordStoreDataTypeList>();
        }

        ~MyDB()
        {
            C?.CloseAsync();
        }

        internal static long GetDBFileSize()
        {
            return new FileInfo(GetDBFilePath()).Length;
        }
        public static string BACKUP_DIRECTORY { get; set; }

        public static string GetBackupDirectory()
        {
            return Path.Combine(BACKUP_DIRECTORY, "data", "com.capersol.recordstore");
        }

        public static string GetBackupFilePath()
        {
            return Path.Combine(GetBackupDirectory(), _DB_BACKUP_NAME);
        }

        internal async static Task<bool> CreateBackup()
        {
            try
            {
                if (!Directory.CreateDirectory(GetBackupDirectory()).Exists)
                {
                    return false;
                }

                await C.BackupAsync(GetBackupFilePath());

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal async static Task<bool> RestoreBackup()
        {
            var sourcePath = GetBackupFilePath();
            var destinationPath = GetDBFilePath();

            try
            {
                if (!File.Exists(sourcePath))
                {
                    return false;
                }

                await C.CloseAsync();

                var db = new MyDB();

                C = db.GetConnection(sourcePath);

                await C.BackupAsync(destinationPath);

                await C.CloseAsync();

                C = db.GetConnection();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
