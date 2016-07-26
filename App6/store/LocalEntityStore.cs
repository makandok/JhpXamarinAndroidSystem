using JhpDataSystem.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {
        public string ConnectionString;
        LocalDB _localDb;
        public LocalDB InstanceLocalDb { get { return _localDb; } }
        internal TableStore defaultTableStore { get; set; }

        public static bool RebuildIndexes { get; set; }

        public LocalEntityStore()
        {
            _localDb = new LocalDB();
            defaultTableStore = new TableStore(Constants.KIND_DEFAULT);
            RebuildIndexes = false;
        }

        public static void RebuildRecordSummaryIndexes()
        {
            var db = new LocalDB3().DB;
            db.DeleteAll<RecordSummary>();
            db.DropTable<RecordSummary>();
            db.CreateTable<RecordSummary>();

            var ppxKinds = Constants.PPX_KIND_DISPLAYNAMES.Keys;
            var vmmcKinds = Constants.VMMC_KIND_DISPLAYNAMES.Keys;
            var combined = new List<string>();
            combined.AddRange(ppxKinds);
            combined.AddRange(vmmcKinds);

            var multiStore = new MultiTableStore()
            {
                Kinds = (from kind in combined select new KindName(kind)).ToList()
            };
            var kindRecords = multiStore.getRecordBlobs();
            var recordSummaries = new List<RecordSummary>();
            foreach (var record in kindRecords)
            {
                var saveable = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<GeneralEntityDataset>(record.Value);
                var editDateObj = saveable.GetValue(Constants.FIELD_PPX_DATEOFVISIT);
                editDateObj = editDateObj ?? saveable.GetValue(Constants.FIELD_VMMC_DATEOFVISIT);
                DateTime dateEdited;
                if (editDateObj == null || string.IsNullOrWhiteSpace(editDateObj.Value))
                {
                    dateEdited = DateTime.MinValue;
                }
                else
                {
                    if (!DateTime.TryParse(editDateObj.Value, out dateEdited))
                    {
                        dateEdited = DateTime.MinValue;
                    }
                }

                var recSummary = new RecordSummary()
                {
                    Id = saveable.Id.Value,
                    EntityId =
                    saveable.EntityId == null ?
                    saveable.Id.Value
                    : saveable.EntityId.Value,
                    VisitDate = dateEdited,
                    KindName = saveable.FormName
                };
                recordSummaries.Add(recSummary);
            }
            db.InsertAll(recordSummaries);
        }

        public static void RebuildClientSummaryIndexes<T, U>(KindName name) where T : class, ILocalDbEntity, new() where U: ClientLookupProvider<T>, new()
        {
            var db = new LocalDB3().DB;
            db.DeleteAll<T>();
            db.DropTable<T>();
            db.CreateTable<T>();

            var vmmcReg = new TableStore(name.Value).GetAllBlobs();
            var allSummaries = new List<T>();
            foreach(var reg in vmmcReg)
            {
                var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneralEntityDataset>(reg.Value);
                var mySummary = new T().Load(entity) as T;
                allSummaries.Add(mySummary);
            }
            new U().InsertOrReplace(allSummaries);
        }

        //protected virtual void saveClientSummary(List<NameValuePair> data, KindKey clientId)
        //{
        //    var clientSummary = new GeneralEntityDataset()
        //    {
        //        Id = clientId,
        //        FormName = _kindName.Value,
        //        EntityId = clientId,
        //        FieldValues = getIndexedFormData(data)
        //    };

        //    var ppclient = new T().Load(clientSummary) as T;
        //    new LocalDB3().DB.InsertOrReplace(ppclient);
        //}

        public static void buildTables()
        {
            var localdb3 = new LocalDB3();
            var db = localdb3.DB;
            db.CreateTable<projects.ppx.PPClientSummary>();
            db.CreateTable<OutEntity>();           
            db.CreateTable<projects.vmc.VmmcClientSummary>();
            db.CreateTable<RecordSummary>();

            db.CreateTable<DeviceConfiguration>();
            //we load from kinds
            if (RebuildIndexes)
            {
                RebuildClientSummaryIndexes<
                    projects.vmc.VmmcClientSummary,
                    projects.vmc.VmmcLookupProvider>(
                    new KindName(Constants.KIND_VMMC_REGANDPROCEDURE));

                RebuildClientSummaryIndexes<
                    projects.ppx.PPClientSummary,
                    projects.ppx.PpxLookupProvider>(
                    new KindName(Constants.KIND_PPX_CLIENTEVAL));

                RebuildRecordSummaryIndexes();
            }

            new TableStore(Constants.KIND_DEFAULT).build();
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

        internal KindKey Save(KindKey entityId, KindName entityKind, KindItem dataToSave)
        {
            //we save to both kindregister and entity table
            return new TableStore(entityKind).Update(entityId, dataToSave);
        }
    }
}