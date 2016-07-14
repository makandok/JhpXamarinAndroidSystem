using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem.store;
using System;

namespace JhpDataSystem.projects.sample
{
    //use this in case you have to load a client in a list for selection
    //e.g. to show first name, surname, date of bith etc
    public class MyLookupInfoProvider
    {
        public List<MyClientLookupInfo> Get()
        {
            var all = new LocalDB3().DB
                .Table<MyClientLookupInfo>()
                .ToList();
            return all;
        }

        public int Update(List<MyClientLookupInfo> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
            return all;
        }
    }

    [SQLite.Table("clientlookupinfotable")]
    public class MyClientLookupInfo
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return Id.GetHashCode(); } set {  } }

        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}