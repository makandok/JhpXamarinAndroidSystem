using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JhpDataSystem.model
{
    public class PPDeviceSizes
    {
        public PPDeviceSizes(int dayId)
        {
            DayId = dayId;
            A = B = C = D = E = 0;
        }
        public int DayId { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }

        public static string getHeader()
        {
            return "Day,\tA,\tB,\tC,\tD,\tE";
        }

        public string toDisplay()
        {
            return string.Format("{0},\t{1},\t{2},\t{3},\t{4},\t{5}", DayId, A, B, C, D, E);
        }

        internal void Add(string deviceSize)
        {
            if (string.IsNullOrWhiteSpace(deviceSize))
                return;

            var asLower = deviceSize.ToLowerInvariant();
            if (asLower == "a")
                A += 1;
            else if (asLower == "b")
                B += 1;
            else if (asLower == "c")
                C += 1;
            else if (asLower == "d")
                D += 1;
            else if (asLower == "e")
                E += 1;
        }
    }

    public class ContactNumber
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
    }

    [SQLite.Table(Constants.KIND_OUTTRANSPORT)]
    public class OutEntity
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int FormSerial { get; set; }
        public string DataBlob { get; set; }
    }

    [SQLite.Table(Constants.KIND_PPX_CLIENTSUMMARY)]
    public class PPClientSummary: ISaveableEntity
    {
        [SQLite.PrimaryKey]
        public int FormSerial { get; set; }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }

        [SQLite.Ignore]
        public KindKey Id { get; set; }
        [SQLite.Ignore]
        public KindKey EntityId { get; set; }

        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.Value.GetHashCode();
            }
            return _itemid;
        }

        public string DeviceSize { get; set; }

        public string KindKey { get; set; }

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
                new NameValuePair() {Name=Constants.FIELD_PLACEMENTDATE,
                    Value = PlacementDate.ToString(CultureInfo.InvariantCulture) },
                new NameValuePair() {Name=Constants.FIELD_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_CLIENTNAME,Value =Names },
                //new NameValuePair() {Name=Constants.FIELD_CARD_SERIAL,Value = FormSerial.ToString() },
                new NameValuePair() {Name=Constants.FIELD_CLIENTIDNUMBER,Value =ClientNumber.ToString() },
                new NameValuePair() {Name=Constants.FIELD_CLIENTTEL,Value = Telephone },
                new NameValuePair() {Name=Constants.FIELD_CLIENTPHYSICALADDR,Value = Address }
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
    }

    public class PPDataSet : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }

        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }

        public PPDataSet fromJson(KindItem prepexDatasetString)
        {
            var pp = JsonConvert.DeserializeObject<PPDataSet>(prepexDatasetString.Value);
            FormName = pp.FormName;
            FieldValues = pp.FieldValues;
            var id = FieldValues.FirstOrDefault(t => t.Name == Constants.FIELD_ID);
            Id = new KindKey(id.Value);
            var entityId = FieldValues.FirstOrDefault(t => t.Name == Constants.FIELD_ENTITYID);
            if (entityId == null && pp.FormName == Constants.KIND_PPX_CLIENTEVAL)
                entityId = id;
            EntityId = new KindKey(entityId.Value);
            return this;
        }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }
        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }
}