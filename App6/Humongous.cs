using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Widget;
using Java.Util;
using JhpDataSystem.model;
using JhpDataSystem.store;
using System;
using System.Collections.Generic;
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
                    _instance = new AppInstance();
                }
                return _instance;
            }
        }

        AssetManager _assetManager { get; set; }
        Activity _mainContext;
        LocalEntityStore _localEntityStoreInstance { get; set; }

        public Dictionary<int, List<FieldValuePair>> TemporalViewData = null;

        public Dictionary<string, string> ApiAssets = null;
        public void InitialiseAppResources(AssetManager assetManager, Activity context)
        {
            _assetManager = assetManager;
            _mainContext = context;
            TemporalViewData = new Dictionary<int, List<FieldValuePair>>();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file
            var inputStream = assetManager.Open(Constants.API_KEYFILE);
            var jsonObject = System.Json.JsonValue.Load(inputStream);

            foreach(var assetName in Constants.ASSET_LIST)
            {
                ApiAssets[assetName] = 
                    jsonObject.decryptAndGetApiSetting(assetName);
            }

            //we need to have this class initialised
            _localEntityStoreInstance = LocalEntityStore.Instance;

            CloudDbInstance = new CloudDb(_assetManager);
            CurrentProjectContext = ProjectContext.None;
        }

        internal ProjectContext CurrentProjectContext { get; private set; }
        internal void SetProjectContext(ProjectContext ppx)
        {
            CurrentProjectContext = ppx;
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

        List<FieldItem> _vmmcFieldItems = null;
        public List<FieldItem> VmmcFieldItems
        {
            get
            {
                if (_vmmcFieldItems == null)
                    _vmmcFieldItems =
                        readFields(Constants.FILE_VMMC_FIELDS, _assetManager, _mainContext);
                return _vmmcFieldItems;
            }
        }

        List<FieldItem> _ppxFieldItems = null;
        public List<FieldItem> PPXFieldItems
        {
            get
            {
                if (_ppxFieldItems == null)
                    _ppxFieldItems =
                        readFields(Constants.FILE_PPX_FIELDS, _assetManager, _mainContext);
                return _ppxFieldItems;
            }
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
    }

    public class DatePickerFragment : DialogFragment,
                                      DatePickerDialog.IOnDateSetListener
    {
        //https://developer.xamarin.com/guides/android/user_interface/date_picker/
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
        Action<DateTime> _dateSelectedHandler = delegate { };
        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            return new DatePickerFragment()
            {
                _dateSelectedHandler = onDateSelected
            };
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = 
                new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month,
                                                           currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            _dateSelectedHandler(selectedDate);
        }
    }
}