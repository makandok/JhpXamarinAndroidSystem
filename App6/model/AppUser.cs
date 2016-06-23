using System;
using System.Collections.Generic;
using System.Linq;

namespace JhpDataSystem.model
{
    [SQLite.Table(Constants.KIND_PREPEX_CLIENT)]
    public class KindRegisterItem:ISaveableEntity
    {
        [SQLite.Ignore]
        public KindKey Id { get; set; }
        public string KindKey { get; set; }
        public string Name { get; set; }
        public string BlobOrFlat { get; set; }
    }

    [SQLite.Table(Constants.KIND_PREPEX_CLIENT)]
    public class PrepexClient: ISaveableEntity
    {
        [SQLite.PrimaryKey]
        public int FormSerial { get; set; }

        [SQLite.Ignore]
        public KindKey Id { get; set; }
        public string Names { get; set; }
        public int ClientNumber { get; set; }

        public DateTime PlacementDate { get; set; }
        public string Telephone { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
    }

    [SQLite.Table(Constants.KIND_PREPEX_CLIENTSUMMARY)]
    public class PrepexClientSummary: ISaveableEntity
    {
        [SQLite.PrimaryKey]
        public int FormSerial { get; set; }

        [SQLite.Ignore]
        public KindKey Id { get; set; }        
        
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

        public PrepexClient GetClientDetails()
        {
            return new PrepexClient()
            {
                Id = this.Id,
                FormSerial = this.FormSerial,
                Names = this.Names,
                ClientNumber = this.ClientNumber,
                PlacementDate = this.PlacementDate,
                Telephone = this.Telephone,
                ContactPhone = this.ContactPhone,
                Address = this.Address
            };
        }

        internal PrepexClientSummary Load(PrepexDataSet lookupEntry)
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
            this.Id = lookupEntry.Id;
            this.PlacementDate = Convert.ToDateTime(allFields["dateofvisit"]);
            this.FormSerial = Convert.ToInt32(allFields["cardserialnumber"]);
            this.Names = allFields["clientname"];
            this.ClientNumber = Convert.ToInt32(allFields["clientidnumber"]);
            this.KindKey = Id.Value;
            this.Telephone = allFields["clienttel"];
            this.Address = allFields["clientsphysicaladdress"];
            return this;
        }
    }

    public class AppUser : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public string UserId { get; set; }
        public string Names { get; set; }
        public string KnownBolg { get; set; }
        //public string MotherOfBolg { get; set; }
    }

    public class PrepexDataSet : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }
        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }
}