using System;
//using Mono.Data.Sqlite;
using System.Collections.Generic;
using JhpDataSystem.model;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using SyncManager.store;
//using SqliteException = System.Data.SqlClient.SqlException;

namespace JhpDataSystem.store
{
    public class CloudLocalStore : BaseTableStore
    {
        public CloudLocalStore(KindName tableName) : base(tableName.Value)
        {
            createKindSql = "if object_id('{0}') is null create table {0}(id nvarchar(32) primary key, entityid nvarchar(32), kindmetadata nvarchar(1000), editday int, editdate bigint, datablob nvarchar(max));";
            insertSql =
                @"
                begin tran
                   update {0} with (serializable) set entityid=@entityid,kindmetadata=@kindmetadata,editday=@editday, editdate=@editdate,datablob =@datablob
                   where id = @id
                   if @@rowcount = 0
                   begin
                      insert into {0} (id,entityid,kindmetadata,editday,editdate,datablob)values(@id,@entityid,@kindmetadata,@editday,@editdate,@datablob);
                   end
                commit tran
                ";
//fix above to include editday and editdate
        }

        /// <summary>
        /// Replaces record in database if it exists or inserts a new one if it doesn't exist. Maintains key supplied
        /// </summary>
        /// <param name="key">Unique id of entity</param>
        /// <param name="dataToSave">Data to save</param>
        /// <returns>Entity Id if successful or default error code</returns>
        public KindKey Update(CloudEntity entity)
        {
            var saved = Save(entity);
            if (!saved)
            {
                return new KindKey(Constants.DBSAVE_ERROR);
            }
            return entity.Id.toKindKey();
        }

        private bool Save(CloudEntity entity)
        {
            //we create if does not exist
            var saveStatus = false;
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    //check if our table tables, create if it doesn't
                    command.CommandText = string.Format(insertSql, _tableName.Value);
                    command.Parameters.AddWithValue("@id", entity.Id);
                    command.Parameters.AddWithValue("@entityid", entity.EntityId);

                    command.Parameters.AddWithValue("@editdate", entity.EditDate);
                    command.Parameters.AddWithValue("@editday", entity.EditDay);

                    command.Parameters.AddWithValue("@datablob", entity.DataBlob);

                    command.Parameters.AddWithValue("@kindmetadata", entity.KindMetaData);
                    //command.Parameters.AddWithValue("@datablob", entity.Id);

                    saveStatus = command.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return false;
                }

                conn.Close();
            }
            return saveStatus;
        }

        internal CloudEntity GetLatestEntity()
        {
            var entity = new CloudEntity();
            using (var conn = new SqlConnection(_db.ConnectionString))
            using (var command = new SqlCommand(string.Format(insertSql, _tableName.Value), conn))
            {
                try
                {
                    conn.Open();
                    //check if our table tables, create if it doesn't
                    command.CommandText = string.Format(
                        "select top 1 * from {0} order by editdate desc", _tableName.Value);                    
                    using(var reader = command.ExecuteReader()  )
                    {
                        while (reader.Read())
                        {
                            entity.Id = Convert.ToString(reader["id"]);
                            entity.EntityId = Convert.ToString(reader["entityid"]);

                            //entity.DateAdded = Convert.ToDateTime(reader["dateadded"]);

                            entity.EditDate = Convert.ToInt64(reader["editdate"]);
                            entity.EditDay = Convert.ToInt32(reader["editday"]);

                            entity.DataBlob = Convert.ToString(reader["datablob"]);
                            entity.KindMetaData = Convert.ToString(reader["kindmetadata"]);
                            break;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (MainLogger != null)
                        MainLogger.Log(string.Format(
                            "Error opening database connection{0}{1}", Environment.NewLine, ex.ToString()));
                    return null;
                }
                conn.Close();
            }
            return entity;
        }
    }
}
