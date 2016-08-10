namespace JhpDataSystem.model
{
    public class FieldItem
    {
        public string name { get; set; }
        public string dataType { get; set; }
        public string pageName { get; set; }
        public bool IsIndexed { get; set; }
        public bool IsRequired { get; set; }
        public string Label { get; set; }
        public int PageId { get; set; }
        public string fieldType { get; set; }
    }

    public class FieldValuePair
    {
        public FieldItem Field { get; set; }
        public string Value { get; set; }

        public NameValuePair AsNameValuePair()
        {
            return new NameValuePair() { Name = Field.name, Value = Value };
        }

        public NameValuePair AsLabelValuePair()
        {
            return new NameValuePair() { Name = Field.Label, Value = Value };
        }
    }

    public class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }

        internal string toDisplayText()
        {
            return Name + ",\t" + Value;
        }
        internal static string getHeaderText()
        {
            return "Name,\tValue";
        }
    }
}