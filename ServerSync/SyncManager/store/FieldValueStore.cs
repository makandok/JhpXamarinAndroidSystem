using JhpDataSystem.model;
using JhpDataSystem.store;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace SyncManager.store
{
    public class FieldValueStore: BaseTableStore
    {
        string _kindName;
        public FieldValueStore(string kindName):base(kindName)
        {
            _kindName = kindName;
            //check if the table exists
            //create if it doesn't
            createKindSql = "if object_id('{0}') is null create table {0}(recordid int not null, fieldname nvarchar(255), fieldvalue nvarchar(255));";
            initialise();
        }

        public int batchSize { get; set; }
        DataTable _fieldValues = null;

        int _currentRecs = 0;
        public void initialise(List<FieldItem> fields = null)
        {
            if (_fieldValues != null)
            {
                _fieldValues.Clear();
            }
            _currentRecs = 0;
            var myFieldValues = new DataTable();
            myFieldValues.Columns.Add(new DataColumn("recordid", typeof(int)));
            myFieldValues.Columns.Add(new DataColumn("fieldname", typeof(string)));
            myFieldValues.Columns.Add(new DataColumn("fieldvalue", typeof(string)));
            _fieldValues = myFieldValues;
        }

        object _localLock = new object();
        public bool Save(GeneralEntityDataset entity, LocalEntity localEntity, int recordId)
        {
            lock (_localLock)
            {
                addToCache(entity, localEntity, recordId);
                _currentRecs++;

                if (_currentRecs >= batchSize)
                {
                    //we save
                    finalise();
                }                
            }
            return true;
        }

        public void finalise()
        {
            saveToDb();
            initialise();
        }

        public bool addToCache(GeneralEntityDataset entity, LocalEntity localEntity, int recordId)
        {
            foreach (var fv in entity.FieldValues)
            {
                _fieldValues.Rows.Add(
                    recordId, fv.Name, fv.Value);
            }

            return true;
        }

        void saveToDb()
        {
            using (var sqlBulkCopy = new SqlBulkCopy(_db.ConnectionString)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = _kindName
            })
            {
                sqlBulkCopy.WriteToServer(_fieldValues);
                //try
                //{
                //    sqlBulkCopy.WriteToServer(_fieldValues);
                //}
                //catch(Exception ex)
                //{

                //}
                
                sqlBulkCopy.Close();
            }
        }
    }
}
