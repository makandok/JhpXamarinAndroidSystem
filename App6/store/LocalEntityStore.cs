using JhpDataSystem.model;
using System.Collections.Generic;
using System;

namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {
        public string ConnectionString;
        static LocalEntityStore _instance;
        LocalDB _localDb;
        public static LocalEntityStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalEntityStore();
                    _instance.Initialise();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Call to ensure that all relevant tables are built
        /// </summary>
        private void Initialise()
        {
            _localDb = new LocalDB();
            ConnectionString = new LocalDB().ConnectionString;

            defaultTableStore = new TableStore(Constants.KIND_DEFAULT);
            defaultTableStore.build();
            new TableStore(Constants.KIND_APPUSERS).build();

            //prepex clients
            new TableStore(Constants.KIND_PREPEX).build();
            new TableStore(Constants.KIND_PREPEX_CLIENTEVAL).build();
            new TableStore(Constants.KIND_PREPEX_DEVICEREMOVAL).build();
            new TableStore(Constants.KIND_PREPEX_POSTREMOVAL).build();
            new TableStore(Constants.KIND_PREPEX_UNSCHEDULEDVISIT).build();
            //new TableStore(Constants.KIND_PREPEX_CLIENTEVAL).build();

            new TableStore(Constants.KIND_VMMC).build();
            new KindRegistryQuery().build();
            //new TableStore(Constants.KIND_VMMC_POSTOP).build();
        }

        internal TableStore defaultTableStore { get; set; }

        public LocalDB InstanceLocalDb { get { return _localDb; } }

        //internal KindKey Put(KindName entityKind, KindItem dataToSave)
        //{
        //    return new TableStore(entityKind).Put(dataToSave);
        //}

        internal KindKey Save(KindKey entityId, KindName entityKind, KindItem dataToSave)
        {
            //we save to both kindregister and entity table
            return new TableStore(entityKind).Update(entityId, dataToSave);
        }

        public List<KindKey> GetKeys(KindName entityKind)
        {
            return new TableStore(entityKind).GetKeys();
        }

        public List<KindItem> Get(KindKey entityId, KindName entityKind)
        {
            return new TableStore(entityKind).Get(entityId);
        }

        public List<KindItem> Get(KindKey entityId)
        {
            var register = new KindRegistryQuery().GetKindForKey(entityId);
            if (register.Value != Constants.DBSAVE_ERROR)
            {
                return Get(entityId);
            }
            return null;
        }

        public List<KindItem> GetAllBlobs(KindName entityKind)
        {
            return new TableStore(entityKind).GetAllBlobs();
        }
    }


}