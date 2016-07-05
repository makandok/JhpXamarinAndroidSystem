using System;

namespace JhpDataSystem.model
{
    public interface ISaveableEntity
    {
        KindKey Id { get; set; }
        KindKey EntityId { get; set; }
    }

    public class KindEntity
    {
        public KindItem kindItem { get; set; }
        public KindName kindName { get; set; }
        public KindKey kindKey { get; set; }
        //public KindEntity()
        //{
        //    //_kindItem = kindItem;
        //    //_kindKey = kindKey;
        //    //_kindName = kindName;
        //}
    }

    public class KindItem
    {
        public KindItem(string value)
        {
            Value = value;
        }
        public string Value;
    }

    public class KindKey
    {
        public KindKey(string id)
        {
            Value = id;
        }
        public string Value;
    }

    public class KindName
    {
        public KindName(string name)
        {
            Value = name;
        }
        public string Value;
    }
}