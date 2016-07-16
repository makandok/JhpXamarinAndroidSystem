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

    [SQLite.Table(Constants.KIND_PPX_CLIENTSUMMARY)]
    public class PPClientSummary: ILocalDbEntity
    {
        public const string KindName = Constants.KIND_PPX_CLIENTSUMMARY;

        [SQLite.Ignore]
        public KindKey Id { get; set; }
        [SQLite.Ignore]
        public KindKey EntityId { get; set; }

        public DateTime CoreActivityDate { get; set; }
        public ISaveableEntity build()
        {
            Id = new KindKey(KindKey); EntityId = new KindKey(KindKey);
            CoreActivityDate = PlacementDate;
            return this;
        }

        [SQLite.PrimaryKey]
        public int FormSerial { get; set; }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }

        public string KindKey { get; set; }

        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.Value.GetHashCode();
            }
            return _itemid;
        }

        public string DeviceSize { get; set; }

        public string Names { get; set; }
        public int ClientNumber { get; set; }

        public DateTime PlacementDate { get; set; }
        public string Telephone { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }

        public DateTime Day3PhoneCallDate { get; set; }
        public DateTime Day6SmsReminderDate { get; set; }
        public DateTime Day7RemovalPhoneCall { get; set; }
        public DateTime Day9HomeVisitDate { get; set; }
        public DateTime Day14FollowupCallDate { get; set; }
        public DateTime Day49CallDate { get; set; }
        public DateTime Day56PhoneCall { get; set; }

        internal List<NameValuePair> ToValuesList()
        {
            var toReturn = new List<NameValuePair>()
            {
                new NameValuePair() {Name=Constants.FIELD_ENTITYID,Value = EntityId.Value },
                new NameValuePair() {Name=Constants.FIELD_ID,Value = Id.Value },
                new NameValuePair() {Name=Constants.FIELD_PPX_PLACEMENTDATE,
                    Value = PlacementDate.ToString(CultureInfo.InvariantCulture) },
                new NameValuePair() {Name=Constants.FIELD_PPX_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTNAME,Value =Names },
                //new NameValuePair() {Name=Constants.FIELD_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTIDNUMBER,Value =ClientNumber.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTTEL,Value = Telephone },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTPHYSICALADDR,Value = Address }
            };
            return toReturn;
        }

        internal PPClientSummary Load(PPDataSet lookupEntry)
        {
            var expectedFields = Constants.PP_IndexedFieldNames;
            var fieldValues = lookupEntry.FieldValues;
            var allFields = (from field in lookupEntry.FieldValues
                             where expectedFields.Contains(field.Name)
                             select field).ToDictionary(x=>x.Name, y=>y.Value);
            foreach(var field in expectedFields)
            {
                if (!allFields.ContainsKey(field))
                {
                    allFields[field] = "";
                }
            }

            this.KindKey = lookupEntry.Id.Value;
            this.EntityId = new KindKey(KindKey);
            this.Id = lookupEntry.Id;

            var deviceSize = string.Empty;
            if(allFields.TryGetValue(Constants.FIELD_PPX_DEVSIZE, out deviceSize))
            {
                this.DeviceSize = deviceSize;
            }

            this.PlacementDate = Convert.ToDateTime(allFields["dateofvisit"]);
            this.FormSerial = Convert.ToInt32(allFields["cardserialnumber"]);
            this.Names = allFields["clientname"];
            this.ClientNumber = Convert.ToInt32(allFields["clientidnumber"]);
            this.Telephone = allFields["clienttel"];
            this.Address = allFields["clientsphysicaladdress"];
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
        //public ISaveableEntity build()
        //{
        //    return this;
        //}
    }

    public class PPDataSet : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }
        //public ISaveableEntity build()
        //{
        //    return this;
        //}
        public PPDataSet fromJson(KindItem prepexDatasetString)
        {
            var pp = JsonConvert.DeserializeObject<PPDataSet>(prepexDatasetString.Value);
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
                    if (pp.FormName == Constants.KIND_PPX_CLIENTEVAL)
                        EntityId = new KindKey(Id.Value);
                }
            }
            return this;
        }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }

        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }

    [SQLite.Table(Constants.KIND_VMMC_CLIENTSUMMARY)]
    public class VmmcClientSummary : ILocalDbEntity
    {
        [SQLite.Ignore]
        public KindKey Id { get; set; }
        [SQLite.Ignore]
        public KindKey EntityId { get; set; }

        public DateTime CoreActivityDate { get; set; }
        public static string KindName { get { return Constants.KIND_PPX_CLIENTSUMMARY; } }

        public ISaveableEntity build()
        {
            Id = new KindKey(KindKey); EntityId = new KindKey(KindKey);
            CoreActivityDate = MCDate;
            return this;
        }


        [SQLite.PrimaryKey]
        public int FormSerial { get; set; }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }

        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.Value.GetHashCode();
            }
            return _itemid;
        }

        public string KindKey { get; set; }

        public string Names { get; set; }
        public int ClientNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime MCDate { get; set; }

        public DateTime PlacementDate { get; set; }
        
        public string Telephone { get; set; }

        public string Address { get; set; }

        internal List<NameValuePair> ToValuesList()
        {
            var toReturn = new List<NameValuePair>()
            {
                new NameValuePair() {Name=Constants.FIELD_ENTITYID,Value = EntityId.Value },
                new NameValuePair() {Name=Constants.FIELD_ID,Value = Id.Value },
                new NameValuePair() {Name=Constants.FIELD_VMMC_MCDATE,
                    Value = MCDate.ToString(CultureInfo.InvariantCulture) },

                new NameValuePair() {Name=Constants.FIELD_PPX_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTNAME,Value =Names },
                new NameValuePair() {Name=Constants.FIELD_VMMC_DOB,Value = DateOfBirth.ToString(CultureInfo.InvariantCulture) },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTIDNUMBER,Value =ClientNumber.ToString() },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTTEL,Value = Telephone },
                new NameValuePair() {Name=Constants.FIELD_PPX_CLIENTPHYSICALADDR,Value = Address }
            };
            return toReturn;
        }

        internal VmmcClientSummary Load(PPDataSet lookupEntry)
        {
            var expectedFields = Constants.PP_IndexedFieldNames;
            var fieldValues = lookupEntry.FieldValues;
            var allFields = (from field in lookupEntry.FieldValues
                             where expectedFields.Contains(field.Name)
                             select field).ToDictionary(x => x.Name, y => y.Value);
            foreach (var field in expectedFields)
            {
                if (!allFields.ContainsKey(field))
                {
                    allFields[field] = "";
                }
            }

            this.KindKey = lookupEntry.Id.Value;
            this.EntityId = new KindKey(KindKey);
            this.Id = lookupEntry.Id;

            //var deviceSize = string.Empty;
            //if (allFields.TryGetValue(Constants.FIELD_PPX_DEVSIZE, out deviceSize))
            //{
            //    this.DeviceSize = deviceSize;
            //}

            this.PlacementDate = Convert.ToDateTime(allFields["dateofvisit"]);
            this.FormSerial = Convert.ToInt32(allFields["cardserialnumber"]);
            this.Names = allFields["clientname"];
            this.ClientNumber = Convert.ToInt32(allFields["clientidnumber"]);
            this.Telephone = allFields["clienttel"];
            this.Address = allFields["clientsphysicaladdress"];
            return this;
        }
    }
}