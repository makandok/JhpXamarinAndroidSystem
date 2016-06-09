using JhpDataSystem.model;
using System.Collections.Generic;

namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {
        public string ConnectionString;
        static LocalEntityStore _instance;
        public static LocalEntityStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalEntityStore();
                    _instance.initialiseStore();
                }
                return _instance;
            }
        }

        void initialiseStore()
        {
            ConnectionString = new LocalDB().GetDb();
            //does nothing, can remove it
            new KindRegistryQuery().build();
        }

        internal KindKey Put(KindName entityKind, KindItem dataToSave)
        {
            return new TableStore(entityKind).Put(dataToSave);
        }

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