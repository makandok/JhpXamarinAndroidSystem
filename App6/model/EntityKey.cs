using System;

namespace JhpDataSystem.model
{
    public interface ISaveableEntity
    {
        KindKey Id { get; set; }
        //KindName Kind { get; }
    }

    public class KindItem //: ISaveableEntity
    {
        public KindItem(string value)
        {
            Value = value;
        }
        public string Value;

        //public KindKey Id { get; set; }
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