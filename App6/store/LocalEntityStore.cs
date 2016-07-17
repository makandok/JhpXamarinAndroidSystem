using JhpDataSystem.model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {
        public string ConnectionString;
        static LocalEntityStore _instance;
        LocalDB _localDb;
        public static LocalEntityStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalEntityStore();
                    _instance.Initialise();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Call to ensure that all relevant tables are built
        /// </summary>
        private void Initialise()
        {
            _localDb = new LocalDB();
            //ConnectionString = _localDb.ConnectionString;

            var localdb3 = new LocalDB3();
            var db = localdb3.DB;
            db.CreateTable<projects.ppx.PPClientSummary>();
            db.CreateTable<OutEntity>();
            db.CreateTable<projects.vmc.VmmcClientSummary>();

            //db.DeleteAll<RecordSummary>();
            db.CreateTable<RecordSummary>();

            defaultTableStore = new TableStore(Constants.KIND_DEFAULT);
            defaultTableStore.build();
            new TableStore(Constants.KIND_APPUSERS).build();

            //prepex clients
            new TableStore(Constants.KIND_PPX_CLIENTEVAL).build();
            new TableStore(Constants.KIND_PPX_DEVICEREMOVAL).build();
            new TableStore(Constants.KIND_PPX_POSTREMOVAL).build();
            new TableStore(Constants.KIND_PPX_UNSCHEDULEDVISIT).build();

            //VMMC
            new TableStore(Constants.KIND_VMMC_POSTOP).build();
            new TableStore(Constants.KIND_VMMC_REGANDPROCEDURE).build();
        }

        internal TableStore defaultTableStore { get; set; }

        public LocalDB InstanceLocalDb { get { return _localDb; } }

        internal KindKey Save(KindKey entityId, KindName entityKind, KindItem dataToSave)
        {
            //we save to both kindregister and entity table
            return new TableStore(entityKind).Update(entityId, dataToSave);
        }

        //public List<KindKey> GetKeys(KindName entityKind)
        //{
        //    return new TableStore(entityKind).GetKeys();
        //}

        //public List<KindItem> Get(KindKey entityId, KindName entityKind)
        //{
        //    return new TableStore(entityKind).Get(entityId);
        //}

        public List<KindItem> GetAllBlobs(KindName entityKind)
        {
            return new TableStore(entityKind).GetAllBlobs();
        }

        //public void updateRecordSummaryTable()
        //{
        //    var query = "select count(*) from {0}";
        //    var allBlobs = GetAllBobs();
        //    var clientRecords = (from record in allBlobs
        //                         select new PPDataSet().fromJson(record));
        //    var allRecords = (
        //        from record in clientRecords
        //        let val = record.FieldValues
        //            .FirstOrDefault(f => f.Name == Constants.FIELD_PPX_DATEOFVISIT)
        //        where val != null
        //        let visitDate = string.IsNullOrWhiteSpace(val.Value) ? DateTime.MinValue : Convert.ToDateTime(val.Value)
        //        select new RecordSummary()
        //        {
        //            Id = record.Id.Value,
        //            EntityId = record.EntityId.Value,
        //            KindName = record.FormName,
        //            //Constants.PPX_KIND_DISPLAYNAMES[record.FormName],
        //            VisitDate = visitDate
        //        }).ToList();

        //    var db = new LocalDB3().DB;
        //    allRecords.ForEach(t => db.InsertOrReplace(t));
        //    var allSaved = db.Table<RecordSummary>().ToList();
        //}

        public void updateRecordSummaryTable()
        {
            var allBlobs = GetAllBobs();
            var clientRecords = (from record in allBlobs
                                 select new GeneralEntityDataset().fromJson(record));
            var allRecords =(
                from record in clientRecords
                let val = record.FieldValues
                    .FirstOrDefault(f => f.Name == Constants.FIELD_PPX_DATEOFVISIT)
                where val != null
                let visitDate = string.IsNullOrWhiteSpace(val.Value) ? DateTime.MinValue : Convert.ToDateTime(val.Value)
                select new RecordSummary()
                {
                    Id = record.Id.Value,
                    EntityId = record.EntityId.Value,
                    KindName = record.FormName,
                    //Constants.PPX_KIND_DISPLAYNAMES[record.FormName],
                    VisitDate = visitDate
                }).ToList();

            var db = new LocalDB3().DB;
            allRecords.ForEach(t => db.InsertOrReplace(t));
            var allSaved = db.Table<RecordSummary>().ToList();            
        }

        public List<NameValuePair> GetAllBobsCount()
        {
            var tables = new List<string>() {
                    Constants.KIND_PPX_CLIENTEVAL,
                    Constants.KIND_PPX_DEVICEREMOVAL,
                    Constants.KIND_PPX_POSTREMOVAL,
                    Constants.KIND_PPX_UNSCHEDULEDVISIT
                };
            var store = new MultiTableStore()
            {
                Kinds =
                (from table in tables select new KindName(table)).ToList()
            };

            var allBlobs = store.getAllBobsCount();
            return allBlobs;
        }

        public List<KindItem> GetAllBobs()
        {
            var tables = new List<string>() {
                    Constants.KIND_PPX_CLIENTEVAL,
                    Constants.KIND_PPX_DEVICEREMOVAL,
                    Constants.KIND_PPX_POSTREMOVAL,
                    Constants.KIND_PPX_UNSCHEDULEDVISIT
                };
            var store = new MultiTableStore()
            {
                //DisplayKindNameMap = Constants.PPX_KIND_DISPLAYNAMES,
                Kinds =
                (from table in tables select new KindName(table)).ToList()
            };

            var allBlobs = store.getRecordBlobs();
            return allBlobs; 
        }
    }
}