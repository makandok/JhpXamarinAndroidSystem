using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JhpDataSystem
{
    public class AppInstance
    {
        static AppInstance _instance;
        public static AppInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppInstance()
                    {
                        AppVersion = "1.0"
                    }
                        ;
                }
                return _instance;
            }
        }

        AssetManager _assetManager { get; set; }
        Activity _mainContext;
        public LocalEntityStore LocalEntityStoreInstance { get; private set; }

        public Dictionary<int, List<FieldValuePair>> TemporalViewData = null;

        public Dictionary<string, string> ApiAssets = null;
        public void InitialiseAppResources(AssetManager assetManager, Activity context)
        {
            //ContextManager = null;
            ModuleContext = null;
            _assetManager = assetManager;
            _mainContext = context;
            TemporalViewData = new Dictionary<int, List<FieldValuePair>>();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file
            var inputStream = assetManager.Open(Constants.API_KEYFILE);
            var jsonObject = System.Json.JsonValue.Load(inputStream);

            foreach (var assetName in Constants.ASSET_LIST)
            {
                ApiAssets[assetName] =
                    jsonObject.decryptAndGetApiSetting(assetName);
            }

            //we need to have this class initialised
            LocalEntityStore.buildTables();
            LocalEntityStoreInstance = new LocalEntityStore();
            CloudDbInstance = new CloudDb(_assetManager);

            //Android.OS.Build.Serial
            var configuration = new LocalDB3().DB.Table<DeviceConfiguration>().FirstOrDefault();
            Configuration = configuration;
        }

        public projects.ContextLocalEntityStore ModuleContext { get; set; }

        internal void SetProjectContext(projects.BaseContextManager ctxt)
        {
            ModuleContext = new projects.ContextLocalEntityStore(ctxt);
        }

        List<FieldItem> readFields(string fieldsAssetName, AssetManager assetManager, Activity context)
        {
            //we load the fields
            var fieldStream = assetManager.Open(fieldsAssetName);

            var asString = fieldStream.toText();

            var fields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FieldItem>>(asString);

            var viewPages = fields.Select(t => t.pageName).Distinct().ToList();

            var dictionary = new Dictionary<string, int>();
            foreach (var page in viewPages)
            {
                var id = context.Resources.GetIdentifier(page, "layout", context.PackageName);
                if (id == 0) throw new ArgumentOutOfRangeException("Could not determine Id for layout " + page);
                dictionary[page] = id;
            }

            foreach (var field in fields)
            {
                field.PageId = dictionary[field.pageName];
            }

            return fields;
        }

        internal void LogActionItem(string v)
        {
            //todo: implement LogActionItem in AppInstance
            //throw new NotImplementedException();
        }

        //public List<FieldItem> VmmcFieldItems
        //{
        //    get
        //    {
        //        if (ContextManager != null && ContextManager.ProjectCtxt == ProjectContext.Vmmc)

        //            return ModuleContext.ContextManager.FieldItems;
        //        if (ContextManager != null && ContextManager.ProjectCtxt == ProjectContext.Vmmc)
        //            return ContextManager.FieldItems;
        //        throw new ArgumentNullException("Project context not defined");
        //    }
        //}

        //public List<FieldItem> PPXFieldItems
        //{
        //    get
        //    {
        //        if (ContextManager != null && ContextManager.ProjectCtxt == ProjectContext.Ppx)
        //            return ContextManager.FieldItems;
        //        throw new ArgumentNullException("Project context not defined");
        //    }
        //}

        public UserSession CurrentUser { get; internal set; }

        internal void SetTempDataForView(int viewId, List<FieldValuePair> valueFields)
        {
            throw new NotImplementedException();
        }

        public CloudDb CloudDbInstance
        {
            get; private set;
        }
        public DeviceConfiguration Configuration { get; internal set; }
        public string AppVersion { get; internal set; }

        //public class MyAppConfiguration
        //{
        //    public int DeviceId
        //    {
        //        get;set;
        //    }
        //    public int FacilityIndex
        //    {
        //        get; set;
        //    }
        //}
    }

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

        public static void RebuildClientSummaryIndexes<T, U>(KindName name) where T : class, ILocalDbEntity, new() where U : ClientLookupProvider<T>, new()
        {
            var db = new LocalDB3().DB;
            db.DeleteAll<T>();
            db.DropTable<T>();
            db.CreateTable<T>();

            var vmmcReg = new TableStore(name.Value).GetAllBlobs();
            var allSummaries = new List<T>();
            foreach (var reg in vmmcReg)
            {
                var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneralEntityDataset>(reg.Value);
                var mySummary = new T().Load(entity) as T;
                allSummaries.Add(mySummary);
            }
            new U().InsertOrReplace(allSummaries);
        }

        internal void ClearAllData()
        {
            clearTables();
        }

        private static void clearTables()
        {
            buildTables();

            var db = new LocalDB3().DB;
            db.DeleteAll<OutEntity>();
            db.DeleteAll<OutEntityUnsynced>();

            db.DeleteAll<projects.ppx.PPClientSummary>();
            db.DeleteAll<projects.vmc.VmmcClientSummary>();
            db.DeleteAll<RecordSummary>();

            new TableStore(Constants.KIND_DEFAULT).DeleteAll();

            new TableStore(Constants.KIND_SITESESSION).DeleteAll();
            new TableStore(Constants.KIND_SITEPROVIDER).DeleteAll();

            //prepex clients
            new TableStore(Constants.KIND_PPX_CLIENTEVAL).DeleteAll();
            new TableStore(Constants.KIND_PPX_DEVICEREMOVAL).DeleteAll();
            new TableStore(Constants.KIND_PPX_POSTREMOVAL).DeleteAll();
            new TableStore(Constants.KIND_PPX_UNSCHEDULEDVISIT).DeleteAll();

            //VMMC
            new TableStore(Constants.KIND_VMMC_POSTOP).DeleteAll();
            new TableStore(Constants.KIND_VMMC_REGANDPROCEDURE).DeleteAll();
        }

        public static void buildTables()
        {
            var db = new LocalDB3().DB;
            db.CreateTable<OutEntity>();
            db.CreateTable<OutEntityUnsynced>();

            db.CreateTable<projects.ppx.PPClientSummary>();
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

            //new TableStore(Constants.KIND_SITEPROVIDER).build();
            new TableStore(Constants.KIND_SITESESSION).build();
            new TableStore(Constants.KIND_SITEPROVIDER).build();

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
