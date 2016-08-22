using Android.App;
using Android.Content.Res;
using JhpDataSystem.model;
using JhpDataSystem.store;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var bytes = SyncManager.Properties.Resources.api_keys;
            var inputStream = new MemoryStream(bytes).toText();

            //var keys = new Dictionary<string, string>();
            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(inputStream)
                as Newtonsoft.Json.Linq.JContainer;
            var tokens = jObject.ToList<JToken>();
            foreach (var token in tokens)
            {
                var name = token.First();
                var assetName = name.Path;
                var assetValue = name.Value<string>();

                if (Constants.ASSET_LIST.Contains(assetName.ToLowerInvariant()))
                {
                    ApiAssets[assetName] = assetValue;
                }                
            }

            //we need to have this class initialised
            LocalEntityStoreInstance = new LocalEntityStore();
            LocalEntityStoreInstance.buildTables(false);
            CloudDbInstance = new CloudDb() { ApiAssets = ApiAssets };
            foreach(var table in CloudDb.getAllKindNames())
            {
                new CloudLocalStore(table.toKind()).build();
            }
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

            var fields = Newtonsoft.Json.JsonConvert.
                DeserializeObject<List<FieldItem>>(asString);

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

        public UserSession CurrentUser { get; internal set; }

        internal void SetTempDataForView(int viewId, List<FieldValuePair> valueFields)
        {
            throw new NotImplementedException();
        }

        public CloudDb CloudDbInstance
        {
            get;private set;
        }
        public DeviceConfiguration Configuration { get; internal set; }
        public string AppVersion { get; internal set; }
    }
}