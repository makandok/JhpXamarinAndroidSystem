using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JhpDataSystem.model
{
    public class ContactNumber
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
    }

    [SQLite.Table(Constants.KIND_OUTTRANSPORT)]
    public class OutEntity
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }
        public string DataBlob { get; set; }
    }

    [SQLite.Table(Constants.KIND_FAILEDOUTTRANSPORT)]
    public class OutEntityUnsynced
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }
        public string DataBlob { get; set; }
        public OutEntityUnsynced load(OutEntity initial)
        {
            Id = initial.Id;
            DataBlob = initial.DataBlob;
            return this;
        }
    }

    public class KindMetaData
    {
        public string devid { get; set; }
        public int chksum { get; set; }
        public int facidx { get; set; }
        public string getJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        public KindMetaData fromJson(KindItem jsonValue)
        {
            var obj = JsonConvert.DeserializeObject<KindMetaData>(jsonValue.Value);
            devid = obj.devid;
            chksum = obj.chksum;
            facidx = obj.facidx;
            return this;
        }
    }

    public class AppUser : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string UserId { get; set; }
        public string Names { get; set; }
        public string KnownBolg { get; set; }

        public string KindMetaData { get; set; }
        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }
        public GeneralEntityDataset asGeneralEntity(KindName name)
        {
            return new GeneralEntityDataset()
            {
                Id = this.Id,
                EntityId = this.EntityId,

                KindMetaData = this.KindMetaData,

                FormName = name.Value,
                FieldValues = new List<NameValuePair>() {
                    new NameValuePair() {Name=Constants.FIELD_ID, Value=this.Id.Value },
                    new NameValuePair() {Name=Constants.FIELD_ENTITYID, Value=this.EntityId.Value },

                    new NameValuePair() {Name=Constants.SYS_FIELD_USERID, Value=this.UserId },
                    new NameValuePair() {Name=Constants.SYS_FIELD_USERNAMES, Value=this.Names},
                    new NameValuePair() {Name=Constants.SYS_FIELD_PASSWDHASH, Value=this.KnownBolg },
                }
            };
        }
    }

    public class GeneralEntityDataset : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }
        public string KindMetaData { get; set; }

        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }

        internal NameValuePair GetValue(string fieldName)
        {
            var toReturn = FieldValues
                .Where(t => t.Name.Contains(fieldName))
                .FirstOrDefault();
            return toReturn;
        }
        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }

        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }
}