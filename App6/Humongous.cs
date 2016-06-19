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

        public Dictionary<int, List<FieldValuePair>> TemporalViewData = null;

        public Dictionary<string, string> ApiAssets = null;
        public void InitialiseAppResources(AssetManager assets)
        {
            TemporalViewData = new Dictionary<int, List<FieldValuePair>>();
            ApiAssets = new Dictionary<string, string>();
            //we read the api key file
            var inputStream = assets.Open(Constants.API_KEYFILE);
            var jsonObject = System.Json.JsonValue.Load(inputStream);

            ApiAssets[Constants.ASSET_NAME_APPNAME] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_NAME_APPNAME);
            ApiAssets[Constants.ASSET_PROJECT_ID] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_PROJECT_ID);
            ApiAssets[Constants.ASSET_NAME_SVC_ACCTEMAIL] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_NAME_SVC_ACCTEMAIL);
            ApiAssets[Constants.ASSET_DATASTORE_APPKEY] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_DATASTORE_APPKEY);
            ApiAssets[Constants.ASSET_P12KEYFILE] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_P12KEYFILE);
            ApiAssets[Constants.ASSET_ADMIN_HASH] = jsonObject.decryptAndGetApiSetting(Constants.ASSET_ADMIN_HASH);

            //we need to have this class initialised
            var cs = LocalEntityStore.Instance.ConnectionString;

            //we load the fields
            var prepexFieldsStream = assets.Open(Constants.FILE_PREPEX_FIELDS_CLIENTEVAL);
            var asString = prepexFieldsStream.toText();

            var fields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FieldItem>>(asString);
            FieldItems = fields;
            //var inputStream = assets.Open(Constants.API_KEYFILE);
            //var jsonObject = System.Json.JsonValue.Load(inputStream);

        }

        public List<FieldItem> FieldItems = null;

        public UserSession CurrentUser { get; internal set; }

        internal void SetTempDataForView(int viewId, List<FieldValuePair> valueFields)
        {
            throw new NotImplementedException();
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