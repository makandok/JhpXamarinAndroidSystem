using JhpDataSystem.model;

namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {
        public string ConnectionString;
        LocalDB _localDb;
        public LocalDB InstanceLocalDb { get { return _localDb; } }
        internal TableStore defaultTableStore { get; set; }

        public LocalEntityStore()
        {
            _localDb = new LocalDB();
            defaultTableStore = new TableStore(Constants.KIND_DEFAULT);
        }

        public static void buildTables()
        {
            var localdb3 = new LocalDB3();
            var db = localdb3.DB;
            db.CreateTable<projects.ppx.PPClientSummary>();
            db.CreateTable<OutEntity>();
            db.CreateTable<projects.vmc.VmmcClientSummary>();

            //db.DeleteAll<RecordSummary>();
            db.CreateTable<RecordSummary>();

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