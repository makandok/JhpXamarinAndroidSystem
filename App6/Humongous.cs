using Android.App;
using Android.Content.Res;
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

        public Dictionary<string, string> ApiAssets = null;
        public void InitialiseAppResources(AssetManager assets)
        {
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
        }

        public UserSession CurrentUser { get; internal set; }


        //public Dictionary<Type, KindName> KindNames = new Dictionary<Type, KindName>()
        //    {
        //        {typeof(AppUser),new KindName(Constants.KIND_APPUSERS) },
        //    };
    }
}