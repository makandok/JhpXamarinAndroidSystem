using System;
using Mono.Data.Sqlite;

namespace App6.db
{
    internal class DbStore
    {
        const string databaseName = "JhpDefaultDB1.db3";
        const string bucketTableName = "bucketstore1";
        const string tableExistSql = "select count(*) from information_schema.tables where table_name = @nameoftable";
        const string createBucketStoreSql = "create table if not exists {0}(id nvarchar(32) primary key, entitykind nvarchar(50), dateadded bigint, datablob nvarchar(255));";
        const string insertSql = "insert or replace into {0}(id, entitykind, dateadded, datablob) values (@id, @entitykind, @dateadded, @datablob)";

        static string connectionString = "URI=file:JhpDefaultDB.db";
        static bool dbInitialised = false;

        internal static void initialiseDb()
        {
            if (dbInitialised)
                return;

            var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var dbpath = System.IO.Path.Combine(personalFolder, databaseName);
            if (!System.IO.File.Exists(dbpath))
            {
                SqliteConnection.CreateFile(dbpath);
            }
            connectionString = "URI=file:" + dbpath;

            //create the main table
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                //check if our table tables, create if it doesn't
                var command = conn.CreateCommand();
                command.Parameters.AddWithValue("@nameoftable", bucketTableName);
                command.CommandText = string.Format(createBucketStoreSql, bucketTableName);
                command.ExecuteNonQuery();
                conn.Close();
            }

            dbInitialised = true;
        }

        const string selectKeysByKindSql = "select id from {0} where entityKind = @eKind;";
        internal static System.Collections.Generic.List<string> GetKey(string entityKind)
        {
            initialiseDb();
            var toReturn = new System.Collections.Generic.List<string>();
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                //check if our table tables, create if it doesn't
                var command = conn.CreateCommand();
                command.CommandText = string.Format(selectKeysByKindSql, bucketTableName);
                command.Parameters.AddWithValue("@eKind", entityKind);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    toReturn.Add(reader.GetString(0));
                }
            }
            return toReturn;
        }

        internal static string newId()
        {
            return Guid.NewGuid().ToString("N");
        }

        internal static string Save(string entityId, string entityKind, string dataToSave)
        {
            initialiseDb();
            var id = string.IsNullOrWhiteSpace(entityId) ? newId() : entityId;
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                //check if our table tables, create if it doesn't
                var command = conn.CreateCommand();
                command.CommandText = string.Format(insertSql, bucketTableName);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@entityKind", entityKind);
                command.Parameters.AddWithValue("@dateadded", DateTime.Now.ToBinary());
                command.Parameters.AddWithValue("@datablob", dataToSave);
                Convert.ToInt32(command.ExecuteScalar());                
            }
            return id;
        }
    }
}