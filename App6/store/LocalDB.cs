using System;
using Mono.Data.Sqlite;

namespace JhpDataSystem.store
{
    public class LocalDB
    {
        public const string defaultDatabaseName = "JhpDefaultDB1.db3";
        public string ConnectionString { get; set; }
        public string GetDb()
        {
            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, defaultDatabaseName);
            if (!System.IO.File.Exists(dbpath))
            {
                SqliteConnection.CreateFile(dbpath);
            }
            return ConnectionString = "URI=file:" + dbpath;
        }

        public string GetDb(string databaseName)
        {
            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, databaseName + ".db3");
            if (!System.IO.File.Exists(dbpath))
            {
                SqliteConnection.CreateFile(dbpath);
            }
            return ConnectionString = "URI=file:" + dbpath;
        }

        public string newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}