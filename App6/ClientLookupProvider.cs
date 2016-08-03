using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem.store;
using Newtonsoft.Json;

namespace JhpDataSystem
{
    public class TablestoreLookupProvider<T>: ClientLookupProvider<T> where T : class, ILocalDbEntity, new()
    {
        public TablestoreLookupProvider(string kindName) :base(kindName)
        {
        }

        public override List<T> Get()
        {
            //we use tablestore
            var kindItems = new TableStore(_kindName).GetAllBlobs();
            var toReturn = new List<T>();
            foreach (var kindItem  in kindItems)
            {
                var ge = JsonConvert.DeserializeObject<GeneralEntityDataset>(kindItem.Value);
                toReturn.Add(new T().Load(ge) as T);
            }
            return toReturn;
        }

        public override int Update(List<T> clients)
        {            
            foreach (var client in clients)
            {
                var ge = new GeneralEntityDataset()
                {
                    EntityId = client.EntityId,
                    FormName = _kindName,
                    Id = client.Id,
                    FieldValues = client.ToValuesList(),
                    KindMetaData = client.KindMetaData
                };
                var saveable = new DbSaveableEntity(ge);
                saveable.Save();
            }
            return 0;
        }

        public override int InsertOrReplace(List<T> clients)
        {
            return Update(clients);
        }

        public override int GetCount()
        {
            return new TableStore(_kindName).Count().Result;
        }
    }

    public class ClientLookupProvider<T> where T : class, ILocalDbEntity, new()
    {
        protected string _kindName;
        public ClientLookupProvider(string kindName)
        {
            _kindName = kindName;
        }

        public virtual List<T> Get()
        {
            var all = new LocalDB3().DB
                .Table<T>()
                .ToList<T>()
                .OrderByDescending(t => t.CoreActivityDate)
                .ToList();
            all.ForEach(t => { t.build(); });
            return all;
        }
        public virtual int Update(List<T> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
            return all;
        }
        public virtual int InsertOrReplace(List<T> clients)
        {
            var db = new LocalDB3().DB;
            foreach(var client in clients)
                db.InsertOrReplace(client);
            return 0;
        }
        public virtual int GetCount()
        {
            var all = new LocalDB3().DB
                .ExecuteScalar<int>("select count(*) from " + _kindName);
            return all;
        }
    }
}