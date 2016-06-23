//using System;
//using SQLite;
//using JhpDataSystem.model;

//namespace JhpDataSystem.store
//{
//    public class FlatTableStore //: ISimpleCrud 
//    {
//        public void a()
//        {
//            var db = new LocalDB3().DB;
//            db.CreateTable<PrepexClientSummary>();
//        }
//        //public FlatTableStore(KindName kind):base(kind){
//        //    TableType = "flat";
//        //    //SQLitePCL.
//        //    }

//        //static Dictionary<Type, string> dataTypes =
//        //    new Dictionary<Type, string>()
//        //    {
//        //        {typeof(Int64),"bigint"},
//        //        {typeof(Boolean),"bit"},
//        //        {typeof(DateTime),"datetime"},
//        //        {typeof(Byte[]),"varbinary(max)"},
//        //        {typeof(Double),"float"},
//        //        {typeof(Int32),"int"},
//        //        {typeof(Decimal),"decimal(18,5)"},
//        //        {typeof(Single),"float"},
//        //        {typeof(Int16),"int"},
//        //        {typeof(string),"nvarchar(255)"},
//        //        {typeof(Char[]),"varbinary(max)"},
//        //        {typeof(Byte),"tinyint"}
//        //    };

//        //public DataTable GetAll()
//        //{
//        //    DataTable ret = null;
//        //    using (var conn = new SqliteConnection(_db.ConnectionString))
//        //    {
//        //        try
//        //        {
//        //            //check if our table tables, create if it doesn't
//        //            var command = conn.CreateCommand();
//        //            command.CommandText = selectDatablobs;
//        //            var reader = command.ExecuteReader();
//        //            var fieldCount = reader.FieldCount;
//        //            while (reader.Read())
//        //            {

//        //            }
//        //        }
//        //        catch (SqliteException ex)
//        //        {
//        //            if (MainLogger != null)
//        //                MainLogger.Log(string.Format(
//        //                    "Error creating table{0}{1}", System.Environment.NewLine, ex.ToString()));
//        //        }
//        //        finally
//        //        {
//        //            conn.Close();
//        //        }
//        //    }
//        //}

//        //public void build<T>() where T:class
//        //{
//        //    //we get the properties
//        //    var properties = typeof(T).GetProperties();
//        //    //we compile the list of fields and matching db datatypes
//        //    var fieldNames = (from property in properties
//        //                      select new
//        //                      {
//        //                          name = property.Name,
//        //                          datatype =
//        //                      dataTypes[property.GetType()]
//        //                      }).ToList();

//        //    //dynamically generate create table or table verification statements
//        //    var columnDefs = (from field in fieldNames
//        //                      select string.Format("{0} {1}", field.name, field.datatype)).ToList();
//        //    var asString = string.Join(",", columnDefs);

//        //    var tableDefSql = string.Format(createKindSql, asString);
//        //    //execute to create the tables
//        //    using (var conn = new SqliteConnection(_db.ConnectionString))
//        //    {
//        //        try
//        //        {
//        //            //check if our table tables, create if it doesn't
//        //            var command = conn.CreateCommand();
//        //            command.CommandText = tableDefSql;
//        //            command.ExecuteNonQuery();
//        //            //saveStatus = true;
//        //        }
//        //        catch (SqliteException ex)
//        //        {
//        //            if (MainLogger != null)
//        //                MainLogger.Log(string.Format(
//        //                    "Error creating table{0}{1}", System.Environment.NewLine, ex.ToString()));
//        //        }
//        //        finally
//        //        {
//        //            conn.Close();
//        //        }
//        //    }
//        //}

//        //public Dictionary<string, object> GetObjectValues<T>(T tObject)
//        //{
//        //    var values = new Dictionary<string, object>();
//        //    var properties = typeof(T).GetProperties();
//        //    foreach (var property in properties)
//        //    {
//        //        var value = property.GetValue(tObject);
//        //        values.Add(property.Name, value);
//        //    }
//        //    return values;
//        //}

//        //new string createKindSql = "create table if not exists {0}(id nvarchar(32) primary key, {0});";
//        //new string insertSql = "insert or replace into {0}(id, {1}) values (@id, {2})";
//        //new string deleteSql = "delete from {0} where id = @id";
//        //new string selectById = "select * from {0} where id = @id";
//        //new string selectIdsForAll = "select id from {0}";
//        //new string selectDatablobs = "select * from {0}";
//        ////public FlatTableStore(KindName kind) : base(kind)
//        ////{
//        ////}
//    }
//}