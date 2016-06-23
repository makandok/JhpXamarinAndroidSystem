using JhpDataSystem.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        public KindKey Key { get { return wrappedEntity.Id; } set { wrappedEntity.Id = value; } }
        public string getJson()
        {
            return JsonConvert.SerializeObject(wrappedEntity);
        }

        public static T fromJson<T>(KindItem jsonString) where T : ISaveableEntity
        {
            return JsonConvert.DeserializeObject<T>(jsonString.Value);
        }

        public void Save()
        {
            if (wrappedEntity == null)
                throw new ArgumentNullException("Wrapped entity can not be null");

            LocalEntityStore.Instance.Save(wrappedEntity.Id, kindName, new KindItem(
                getJson()
                ));
        }
    }
}