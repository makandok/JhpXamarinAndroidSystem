using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JhpDataSystem.model
{
    [SQLite.Table(Constants.SYS_KIND_RECORDSUMMARY)]
    public class RecordSummary
    {
        public RecordSummary()
        {

        }

        private long _itemid = -1L;
        [SQLite.Ignore]
        public long itemId { get { return _itemid; } set { _itemid = value; } }

        public long getItemId()
        {
            if (_itemid == -1L && Id != null)
            {
                _itemid = Id.GetHashCode();
            }
            return _itemid;
        }

        [SQLite.PrimaryKey]
        [SQLite.Unique]
        public string Id { get; set; }

        public string EntityId { get; set; }

        public string KindName { get; set; }
        public DateTime VisitDate { get; set; }
    }

    public class VisitSummary
    {
        public VisitSummary(
            RecordSummary wrapped, string niceKindName)
        {
            Wrapped = wrapped;
            NiceKindName = niceKindName;
        }

        public RecordSummary Wrapped { get; private set; }
        public string NiceKindName { get; set; }
    }
}