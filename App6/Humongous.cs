using JhpDataSystem.model;
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

        //public Dictionary<Type, KindName> KindNames = new Dictionary<Type, KindName>()
        //    {
        //        {typeof(AppUser),new KindName(Constants.KIND_APPUSERS) },
        //    };
    }
}