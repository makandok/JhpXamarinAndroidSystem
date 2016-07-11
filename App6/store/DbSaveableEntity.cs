using JhpDataSystem.model;
using Newtonsoft.Json;
using System;

namespace JhpDataSystem.store
{
    public class DbSaveableEntity
    {
        ISaveableEntity wrappedEntity;
        public DbSaveableEntity(ISaveableEntity entity)
        {
            wrappedEntity = entity;
        }

        public KindName kindName { get; set; }

        public KindKey Id { get { return wrappedEntity.Id; } set { wrappedEntity.Id = value; } }

        public KindKey EntityId { get { return wrappedEntity.EntityId; } set { wrappedEntity.EntityId = value; } }

        //public KindKey Key { get { return wrappedEntity.Id; } set { wrappedEntity.Id = value; } }
        public string getJson()
        {
            return JsonConvert.SerializeObject(wrappedEntity);
        }

        public string getEncryptedJson()
        {
            return getEncryptedEntity().Value.Value;
        }

        public EncryptedKindEntity getEncryptedEntity()
        {
            var asString = JsonConvert.SerializeObject(wrappedEntity);
            var encrypted = asString.Encrypt();
            return new EncryptedKindEntity(encrypted);
        }

        public static T fromJson<T>(KindItem jsonString) where T : ISaveableEntity
        {
            return JsonConvert.DeserializeObject<T>(jsonString.Value);
        }

        public void Save()
        {
            if (wrappedEntity == null)
                throw new ArgumentNullException("Wrapped entity can not be null");
            Save(new KindItem(getJson()));
        }

        public void Save(KindItem kindItem)
        {
            if (wrappedEntity == null)
                throw new ArgumentNullException("Wrapped entity can not be null");
            LocalEntityStore.Instance.Save(wrappedEntity.Id, kindName, kindItem);
        }
    }
}