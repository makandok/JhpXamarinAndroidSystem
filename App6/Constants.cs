using JhpDataSystem.model;

namespace JhpDataSystem
{
    public class Constants
    {
        //tables in the database
        public const string KIND_APPUSERS = "appusers";
        internal const string KIND_PREPEX = "pp_client";
        internal const string KIND_PREPEX_CLIENTEVAL = "pp_client_eval";
        internal const string KIND_PREPEX_DEVICEREMOVAL = "pp_client_devicerem";
        internal const string KIND_PREPEX_POSTREMOVAL = "pp_client_postrem";
        internal const string KIND_PREPEX_UNSCHEDULEDVISIT = "pp_client_unsched";

        internal const string KIND_VMMC = "vmmcclients";
        public const string KIND_DEFAULT = "generalstore";
        public const string KIND_REGISTER = "kindRegister";

        internal const string ASSET_NAME_APPNAME = "applicationname";
        internal const string ASSET_PROJECT_ID = "projectid";
        internal const string ASSET_NAME_SVC_ACCTEMAIL = "serviceaccountemail";
        internal const string ASSET_API_KEYFILE = "api_keys.json";
        internal const string ASSET_DATASTORE_APPKEY = "datastore_appkey";
        internal const string ASSET_P12KEYFILE = "p12keyfile";
        internal const string ASSET_ADMIN_HASH = "adminhash";
        //
        internal const string API_KEYFILE = "api_keys.json";
        internal static System.Collections.Generic.List<string> 
            ENCRYPTED_ASSETS = new System.Collections.Generic.List<string>() { ASSET_DATASTORE_APPKEY };
        public const string DBSAVE_ERROR = "default error value";

        public const string MOTHER_OFBOLG = "Text to Encrypt. Do NOT CHANGE";
        public const string MOTHER_OFALLBOLGS = "ADMIN Text to Encrypt. Do NOT CHANGE";

        internal const string ADMIN_USERNAME = "admin";

        public static string SUPPORTADMIN_USERNAME = "support";

        //prepexreg_fields
        internal const string FILE_PREPEX_FIELDS_CLIENTEVAL = "prepexreg_fields.json";
        internal const string FILE_PREPEX_FIELDS = "prepex_fields.json";

        
        internal const string PP_VIEWS_1= "prepexreg1";
        internal const string PP_VIEWS_2 = "prepexreg2";
        internal const string PP_VIEWS_3 = "prepexreg3";
        internal const string PP_VIEWS_4 = "prepexreg4";

        internal const string DATEPICKER = "DatePicker";
        internal const string EDITTEXT = "EditText";
        internal const string CHECKBOX = "CheckBox";
        internal const string RADIOBUTTON = "RadioButton";

        internal const string DATE_BUTTON_PREFIX = "dtbtn_";
        internal const string DATE_TEXT_PREFIX = "dttxt_";
        internal const string LABEL_PREFIX = "sylbl_";

        internal const string DEFAULT_CHECKED = "1";
    }
}