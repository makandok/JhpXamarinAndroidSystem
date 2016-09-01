using System;

namespace SyncManager.store
{
    public class BlobEntity
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

    public class CloudEntity: BlobEntity
    {
    }

    public class LocalEntity: BlobEntity
    {
    }

    public class EntityConverter
    {
        public LocalEntity toLocalEntity(CloudEntity entity)
        {
            return new LocalEntity()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                DataBlob = decrypt(entity.DataBlob),
                EditDate = entity.EditDate,
                EditDay = entity.EditDay,
                FormName = entity.FormName,
                KindMetaData = entity.KindMetaData
            };
        }

        public CloudEntity toCloudEntity(LocalEntity entity)
        {
            return new CloudEntity()
            {
                Id = entity.Id,
                EntityId = entity.EntityId,
                DataBlob = encrypt(entity.DataBlob),
                EditDate = entity.EditDate,
                EditDay = entity.EditDay,
                FormName = entity.FormName,
                KindMetaData = entity.KindMetaData
            };
        }

        string encrypt(string textToEncrypt)
        {
            return textToEncrypt;
        }

        string decrypt(string textToDecrypt)
        {
            return textToDecrypt;
        }
    }
}
