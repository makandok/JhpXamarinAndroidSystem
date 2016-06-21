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
        static List<ViewDefinitionBuilder> autoCreatePages(List<FieldDefinition> fields, string MODULE_NAME_PREFIX)
        {
            var fieldCounter = 0;
            var pagedFieldDefinitions = new List<ViewDefinitionBuilder>();
            ViewDefinitionBuilder dataPage = null;
            var pageCounter = 0;
            var sets = fields.Count / TARGET_NUMPAGES;
            var previousFieldGroupName = string.Empty;
            foreach (var field in fields)
            {
                var canAssignToNewPage = true;
                if (field.GridColumn == 5)
                {
                    if (previousFieldGroupName != string.Empty && previousFieldGroupName == field.GroupName)
                    {
                        canAssignToNewPage = false;
                    }

                    if (previousFieldGroupName != field.GroupName)
                    {
                        previousFieldGroupName = field.GroupName;
                    }
                }

                if (canAssignToNewPage)
                {
                    if (fieldCounter % sets == 0 && pageCounter < TARGET_NUMPAGES)
                    {
                        //create a new view
                        var currentDataPage = new ViewDefinitionBuilder() { ViewPageName = MODULE_NAME_PREFIX + (++pageCounter) };
                        pagedFieldDefinitions.Add(currentDataPage);
                        dataPage = currentDataPage;
                    }

                    fieldCounter++;
                }

                dataPage.ViewFields.Add(field);
            }
            return pagedFieldDefinitions;
        }

        static void Main(string[] args)
        {
            var instance = SharedInstance.Instance;
            instance.metaDataProvider = new MetaDataProvider();
            //.metaDataProvider
            var XML_FORMAT = "axml";
            var moduleNamePrefixes = new Dictionary<string, string>()
            {
                {"A1 Client Evaluation and Registration","prepexreg"},
                {"A3 Device Removal Visit or Follow Up","prepexdevremoval"},
                {"A4 Post Removal Visit Assessment","prepexpostremoval"},
                {"A2 Unscheduled or Follow-up Prepex Form","prepexunscheduled"}
            };

            
            var text = File.ReadAllText("LookupChoices.json");
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
            if (fields.Count != 208)
                throw new ArgumentOutOfRangeException("Expected 210 fields");

            //fields.RemoveAll(t=>t.)
            var fieldsToIgnore = new List<string>() { "district" };
            foreach (var fieldname in fieldsToIgnore)
            {
                fields.RemoveAll(t => t.ViewName == fieldname);
            }

            fields.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.ListName))
                {
                    var lookup = lookups.Where(p => p.Name == t.ListName).FirstOrDefault();
                    if (lookup == null)
                        throw new ArgumentNullException("Lookup undefined for "+ t.ListName);
                    t.FieldOptions = lookup;
                }
            }
                );

            var viewPages = new Dictionary<string, ViewDefinitionBuilder>();
            var pageNumbersAll = (fields.Select(
                t => new {Combined = t.FormName+t.ViewPage, FormName = t.FormName, ViewPage = t.ViewPage })).ToList();
            var pageNumbers = pageNumbersAll.Distinct().ToList();
            foreach (var page in pageNumbers)
            {
                var moduleNamePrefix = moduleNamePrefixes[page.FormName];
                var currentDataPage = new ViewDefinitionBuilder() {
                    metaDataProvider = SharedInstance.Instance.metaDataProvider,
                    ViewPageName = moduleNamePrefix + page.ViewPage };
                viewPages[page.Combined] = currentDataPage;
            }

            foreach (var field in fields)
            {
                viewPages[field.FormName + field.ViewPage].ViewFields.Add(field);
            }

            //var pagedFieldDefinitions = autoCreatePages();
            var fieldsLookup = "prepex".ToLowerInvariant() + "_fields.json";
            var stringResoures = "prepex".ToLowerInvariant() + "_string.json";
            var isFirst = true;
            //var addDateTitleResource = true;
            foreach (var page in viewPages.Values)
            {
                //page.metaDataProvider = SharedInstance.Instance.metaDataProvider;
                var pageContents = page.build();
                File.WriteAllText(page.ViewPageName.ToLowerInvariant() + ".axml", pageContents);
                File.WriteAllText(page.ViewPageName.ToLowerInvariant() + ".xml", pageContents);                
            }

            File.WriteAllText(stringResoures, SharedInstance.Instance.metaDataProvider.StringResourcesItems.ToString());

            //we write the field dictionary
            //process after FieldDef.build()
            var allFields = SharedInstance.Instance.metaDataProvider.ModelItems;

            var undefinedPageFields = allFields.Where(t => string.IsNullOrWhiteSpace(t.pageName)).ToList();
            if (undefinedPageFields.Count > 0)
            {
                throw new ArgumentNullException("Page name not defined for one of the field items");
            }

            File.WriteAllText(fieldsLookup, JsonConvert.SerializeObject(allFields));

            //kipeto
            Console.WriteLine("Import completed, press any key to return");
            Console.ReadLine();
        }
    }
}
