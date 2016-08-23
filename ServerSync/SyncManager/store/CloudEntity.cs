using System;

namespace SyncManager.store
{
    public class CloudEntity
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string KindMetaData { get; set; }
        public string DataBlob { get; set; }
        //public DateTime DateAdded { get; set; }
        public string FormName { get; set; }
        public long EditDate { get; internal set; }
        public int EditDay { get; internal set; }
    }
}
