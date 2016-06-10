using JhpDataSystem.model;

namespace JhpDataSystem
{
    internal class Constants
    {
        //tables in the database
        public const string KIND_APPUSERS = "appusers";
        internal const string KIND_PREPEX = "prepexclients";
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

        
    }
}