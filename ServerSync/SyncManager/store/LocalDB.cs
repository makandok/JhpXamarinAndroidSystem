using System;
using System.Data.SqlClient;

namespace JhpDataSystem.store
{
    public class LocalDB
    {
        public const string defaultDatabaseName = "JhpDefaultDB1.db3";
        public string ConnectionString { get; set; }
        public LocalDB()
        {
            //same as LocalDB(string databaseName)

            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, defaultDatabaseName);
            if (!System.IO.File.Exists(dbpath))
            {
                //SqlConnection.CreateFile(dbpath);
            }
            ConnectionString = "URI=file:" + dbpath;
        }

        public LocalDB(string databaseName)
        {
            //check if db exists
            //create if it does not exist

            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, databaseName + ".db3");
            if (!System.IO.File.Exists(dbpath))
            {
                //SqlConnection.CreateFile(dbpath);
            }
            ConnectionString = "URI=file:" + dbpath;
        }

        public string newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}