using JhpDataSystem.model;
using Newtonsoft.Json;
using System;

namespace JhpDataSystem.store
{    
    public class DbPrepexClient : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public string Names { get; set; }
        public int ClientNumber { get; set; }
        public int FormSerial { get; set; }
        public long PlacementDate { get; set; }
        public long Day3PhoneCallDate { get; set; }
        public long Day6SmsReminderDate { get; set; }
        public long Day7RemovalPhoneCall { get; set; }
        public long Day9HomeVisitDate { get; set; }
        public long Day14FollowupCallDate { get; set; }
        public long Day49CallDate { get; set; }
        public long Day56PhoneCall { get; set; }
    }

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