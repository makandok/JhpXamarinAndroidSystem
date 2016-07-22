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

    public class AppUser : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string UserId { get; set; }
        public string Names { get; set; }
        public string KnownBolg { get; set; }
    }

    public class GeneralEntityDataset : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }
        public GeneralEntityDataset fromJson(KindItem jsonKindItem)
        {
            var pp = JsonConvert.DeserializeObject<GeneralEntityDataset>(jsonKindItem.Value);
            FormName = pp.FormName;
            FieldValues = pp.FieldValues;
            Id = pp.Id;
            EntityId = pp.EntityId;

            if (pp.Id ==null)
            {
                var id = FieldValues.FirstOrDefault(t => t.Name == Constants.FIELD_ID);
                if (id != null)
                {
                    Id = new KindKey(id.Value);
                }
                else
                {
                    //we skip this record
                }
            }

            if (pp.EntityId == null)
            {
                var entityId = FieldValues.FirstOrDefault(t => t.Name == Constants.FIELD_ENTITYID);
                if (entityId != null)
                {
                    EntityId = new KindKey(entityId.Value);
                }
                else
                {
                    //we skip this record
                    if (pp.FormName == Constants.KIND_PPX_CLIENTEVAL
                        || pp.FormName == Constants.KIND_VMMC_REGANDPROCEDURE
                        )
                        EntityId = new KindKey(Id.Value);
                }
            }
            return this;
        }

        internal NameValuePair GetValue(string fieldName)
        {
            var toReturn = FieldValues
                .Where(t => t.Name.Contains(fieldName))
                .FirstOrDefault();
            return toReturn;
        }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }

        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }
}