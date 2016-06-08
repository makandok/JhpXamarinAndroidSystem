using System;
using System.Collections.Generic;
using System.Json;

namespace JhpDataSystem
{
    public class FieldValue: Interfaces.IJsonable
    {
        public FieldValue()
        {
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public string getJson()
        {
            return "{\"" + Name + "\":\"" + Convert.ToString(Value.ToString()) + "\"}";
        }
    }

    //public class DataStore
    //{
    //    public DataStore(string jsonString)
    //    {

    //    }

    //    public List<FieldValue> FieldValues { get; set; }
    //}
}