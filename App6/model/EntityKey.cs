using System;

namespace JhpDataSystem.model
{
    public interface ILocalDbEntity: ISaveableEntity
    {
        DateTime CoreActivityDate { get; set; }
        ISaveableEntity build();
    }

    public interface ISaveableEntity
    {
        KindKey Id { get; set; }
        KindKey EntityId { get; set; }
    }

    public class EncryptedKindEntity
    {
        public EncryptedKindEntity()
        {
        }
        public EncryptedKindEntity(EncryptedText name)
        {
            Value = name;
        }
        public EncryptedText Value;
    }

    public class KindEntity
    {
        public KindItem kindItem { get; set; }
        public KindName kindName { get; set; }
        public KindKey kindKey { get; set; }
    }

    public class KindItem
    {
        public KindItem()
        {
        }
        public KindItem(string value)
        {
            Value = value;
        }
        public string Value;
    }

    public class KindKey
    {
        public KindKey()
        {
        }
        public KindKey(string id)
        {
            Value = id;
        }
        public string Value;
    }

    public class KindName
    {
        public KindName()
        {
        }

        public KindName(string name)
        {
            Value = name;
        }
        public string Value;
    }
}