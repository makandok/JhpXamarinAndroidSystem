namespace JhpDataSystem.model
{
    public class FieldItem
    {
        //public int fieldId { get; set; }
        public string name { get; set; }
        public string dataType { get; set; }
        public string pageName { get; set; }
        public bool IsLookup { get; set; }
        //public List<string> options { get; set; }
    }

    public class FieldValuePair
    {
        public FieldItem Field { get; set; }
        public string Value { get; set; }

        public NameValuePair AsNameValuePair()
        {
            return new NameValuePair() { Name = Field.name, Value = Value };
        }
    }

    public class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}