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
                        var currentDataPage = new ViewDefinitionBuilder() { pageName = MODULE_NAME_PREFIX + (++pageCounter) };
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
                    t.FieldOptions = lookup;
                }

            }
                );
            
            var viewPages = new Dictionary<string, ViewDefinitionBuilder>();
            var pageNumbers = fields.Select(t => t.ViewPage);
            foreach(var page in pageNumbers)
            {
                var currentDataPage = new ViewDefinitionBuilder() { pageName = MODULE_NAME_PREFIX + page };
                viewPages[page] = currentDataPage;
            }

            foreach(var field in fields)
            {
                viewPages[field.ViewPage].ViewFields.Add(field);
            }

            //var pagedFieldDefinitions = autoCreatePages();
            var fieldsLookup = MODULE_NAME_PREFIX.ToLowerInvariant() + "_fields.json";
            var stringResoures = MODULE_NAME_PREFIX.ToLowerInvariant() + "_string.json";
            var isFirst = true;
            var addDateTitleResource = true;
            foreach (var page in viewPages.Values)
            {
                page.addDateTitleResource = addDateTitleResource;
                var pageContents = page.build();
                addDateTitleResource = false;
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
            var allFields = (from pageFieldDef in viewPages.Values
                             from fielddef in pageFieldDef.metaDataProvider.ModelItems
                             select fielddef).ToList();
            File.WriteAllText(fieldsLookup, JsonConvert.SerializeObject(allFields));

            //kipeto
            Console.WriteLine("Import completed, press any key to return");
            Console.ReadLine();
        }
    }
}
