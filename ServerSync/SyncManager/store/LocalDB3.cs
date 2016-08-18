using System;
using System.Data.SqlClient;

namespace JhpDataSystem.store
{
    public class OutDb: LocalDB3
    {
        public OutDb():base()
        {
        }
    }

    public class LocalDB3
    {
        public string defaultConnectionString = @"data source=SUPER-LAP\SQL2014;initial catalog=ServerSyncDb;integrated security=true;";
        public LocalDB3(string connString)
        {
            defaultConnectionString = connString;
        }
        public LocalDB3()
        {
        }
        public SqlConnection DB
        {
            get
            {
                return new SqlConnection(defaultConnectionString);
            }
        }

        public string newId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}