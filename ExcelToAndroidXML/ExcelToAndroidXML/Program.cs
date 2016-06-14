using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelToAndroidXML
{
    class Program
    {
        
        const int TARGET_NUMPAGES = 4;

        static void Main(string[] args)
        {
            var XML_FORMAT = "axml";
            var MODULE_NAME_PREFIX = "prepexreg";
            var text = File.ReadAllText("json1.json");
            var lookups = JsonConvert.DeserializeObject<List<FieldChoices>>(text);
            lookups.ForEach(
                t =>
                {
                    t.CleanValues = new List<string>();
                    foreach (var value in t.Values)
                    {
                        t.CleanValues.Add(t.Name + "_" + value.Clean());
                    }
                }
                );

            var fields = JsonConvert.DeserializeObject<List<FieldDefinition>>(File.ReadAllText("FieldDictionary.json"));

            fields.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.ListName))
                {
                    var lookup = lookups.Where(p => p.Name == t.ListName).FirstOrDefault();
                    t.FieldOptions = lookup;
                }

            }
                );

            var fieldCounter = 0;
            var pagedFieldDefinitions = new List<ViewDefinitionBuilder>();
            ViewDefinitionBuilder dataPage = null;
            var pageCounter = 0;
            var sets = fields.Count / TARGET_NUMPAGES;
            foreach (var field in fields)
            {
                if (fieldCounter % sets == 0 && pageCounter < TARGET_NUMPAGES)
                {
                    //create a new view
                    var currentDataPage = new ViewDefinitionBuilder() { pageName = MODULE_NAME_PREFIX + (++pageCounter) };
                    pagedFieldDefinitions.Add(currentDataPage);
                    dataPage = currentDataPage;
                }
                fieldCounter++;
                dataPage.ViewFields.Add(field);
            }

            var fieldsLookup = MODULE_NAME_PREFIX.ToLowerInvariant() + "_fields.json";
            var stringResoures = MODULE_NAME_PREFIX.ToLowerInvariant() + "_string.json";
            var isFirst = true;

            foreach (var page in pagedFieldDefinitions)
            {
                var pageContents = page.build();
                //we create the view and write to it
                File.WriteAllText(page.pageName.ToLowerInvariant()+ ".axml", pageContents);
                File.WriteAllText(page.pageName.ToLowerInvariant() + ".xml", pageContents);

                if (isFirst)
                {
                    isFirst = false;
                    //we write the strings resoures entries
                    File.WriteAllText(stringResoures, page.metaDataProvider.StringResourcesItems.ToString());
                }
                else
                {
                    //we write the strings resoures entries
                    File.AppendAllText(stringResoures, page.metaDataProvider.StringResourcesItems.ToString());
                }
            }

            //we write the field dictionary
            //process after FieldDef.build()
            var allFields = (from pageFieldDef in pagedFieldDefinitions
                             from fielddef in pageFieldDef.metaDataProvider.ModelItems
                             select fielddef).ToList();
            File.WriteAllText(fieldsLookup, JsonConvert.SerializeObject(allFields));

            //kipeto
            Console.WriteLine("Import completed, press any key to return");
            Console.ReadLine();
        }
    }



    static class Extensions
    {
        public static string getStringsEntry(this string stringsEntryName, string stringsEntryText)
        {
            return ("<string name='" + stringsEntryName + "'>" + stringsEntryText + "</string>").Replace("'", "\"");
        }

        public static string Clean(this string value)
        {
            //=SUBSTITUTE( SUBSTITUTE( LOWER(I7)," ",""),"/","_")
            return Regex.Replace(value, "[^a-zA-Z0-9]", "").ToLowerInvariant();
        }
    }

    public class FieldChoices
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public List<string> CleanValues { get; set; }
    }

    public class FieldItem
    {
        //public int fieldId { get; set; }
        public string name { get; set; }
        public string dataType { get; set; }
        public string pageName { get; set; }
        //public List<string> options { get; set; }
    }

    public class FieldDefinition
    {
        public string ViewName { get; set; }
        public string ViewType { get; set; }
        public string DisplayLabel { get; set; }
        public string ListName { get; set; }
        public int GridColumn { get; set; }
        public int Id { get; set; }
        public FieldChoices FieldOptions { get; set; }
    }
}
