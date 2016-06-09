using JhpDataSystem.model;
using Newtonsoft.Json;

namespace JhpDataSystem.store
{
    public class DbSaveableEntity
    {
        ISaveableEntity wrappedEntity;
        public DbSaveableEntity(ISaveableEntity entity)
        {
            wrappedEntity = entity;
        }

        public KindKey Key { get { return wrappedEntity.Id; } set { wrappedEntity.Id = value; } }
        public string getJson()
        {
            return JsonConvert.SerializeObject(wrappedEntity);
        }

        public static T fromJson<T>(KindItem jsonString) where T : ISaveableEntity
        {
            return JsonConvert.DeserializeObject<T>(jsonString.Value);
        }
    }
}