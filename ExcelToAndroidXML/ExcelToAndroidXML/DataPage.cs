using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelToAndroidXML
{
    public class MetaDataProvider
    {
        public MetaDataProvider()
        {
            StringResourcesItems = new StringBuilder();
            ModelItems = new List<FieldItem>();
        }

        public void AddStringResource(string resourceName, string resourceValue)
        {
            StringResourcesItems.AppendLine(string.Format("<string name=\"{0}\">{1}</string>", resourceName, resourceValue));
        }
        public StringBuilder StringResourcesItems { get; set; }
        public List<FieldItem> ModelItems { get; set; }
    }

    public class ViewDefinitionBuilder
    {
        public string pageName { get; set; }
        public List<FieldDefinition> ViewFields { get; set; }
        public MetaDataProvider metaDataProvider = null;
        public ViewDefinitionBuilder()
        {
            ViewFields = new List<FieldDefinition>();
        }

        internal string getFieldList(string datasetLabel)
        {
            return string.Empty;
        }

        public string build()
        {
            if (ViewFields.Count == 0)
            {
                return string.Empty;
            }

            var start = @"<?xml version='1.0' encoding='utf-8'?>
<LinearLayout xmlns:android='http://schemas.android.com/apk/res/android'
    android:orientation='vertical'
    android:layout_width='match_parent'
    android:layout_height='wrap_content'>
<include layout='@layout/nextandprevbuttons' />
{0}
</LinearLayout>
";
            metaDataProvider = new MetaDataProvider();
            var asString = getData(ViewFields);

            //we update the page names
            metaDataProvider.ModelItems.ForEach(t => t.pageName = pageName);

            var builder = new StringBuilder()
            .AppendFormat(start.Replace("'", "\""), asString);
            return builder.ToString();
        }

        string getData(List<FieldDefinition> viewFields)
        {
            var builder = new StringBuilder();
            foreach (var field in viewFields)
            {
                var fieldXml = string.Empty;
                switch (field.ViewType)
                {
                    case "int":
                    case "Cell":
                    case "Number":
                        {
                            fieldXml = getXamlDefinitionForNumber(field);
                            break;
                        }
                    case "Date":
                    case "date":
                    case "DatePicker":
                        {
                            fieldXml = getXamlDefinitionForDate(field);
                            break;
                        }
                    case "Text":
                    case "EditText":
                        {
                            fieldXml = getXamlDefinitionForTextField(field);
                            break;
                        }
                    case "Label":
                    case "TextView":
                        {
                            fieldXml = getXamlDefinitionForLabel(field);
                            break;
                        }
                    case "MultiSelect":
                    case "CheckBox":
                        {
                            fieldXml = (
                                getXamlLabelDefForEnumeratedFields(field) + 
                                string.Join(Environment.NewLine, getXamlLabelDefForCheckBox(field)) + @"");
                            break;
                        }
                    case "SingleSelect":
                    case "RadioGroup":
                        {
                            fieldXml = (
                                getXamlLabelDefForEnumeratedFields(field) +
                                getXamlLabelDefForRadioGroup(field) +
                                string.Join(Environment.NewLine, getXamlLabelDefForRadioButton(field))
                                + @"</RadioGroup >");
                            break;
                        }
                    default:
                        {
                            var fieldype = field.ViewType;
                            throw new ArgumentNullException("Pleasse addlogic for " + field.ViewType);
                        }
                }
                builder.AppendLine(fieldXml);
            }
            return builder.ToString();
        }

        string getXamlDefinitionForNumber(FieldDefinition field)
        {
            var stringsEntryText = field.DisplayLabel;
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"
    <EditText
        android:inputType='number'
        android:layout_width='match_parent'
        android:layout_height='wrap_content'
        android:id='@+id/" + stringsEntryName + @"'
        android:hint='@string/" + stringsEntryName + @"' />
");
            metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "EditText", name = stringsEntryName });
            metaDataProvider.AddStringResource(stringsEntryName, stringsEntryText);
            return fieldXml.Replace("'", "\"");
        }

        string getXamlDefinitionForDate(FieldDefinition field)
        {
            var stringsEntryText = field.DisplayLabel;
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"
    <DatePicker
        android:layout_width='match_parent'
android:layout_height='90dp'
android:calendarViewShown='false'
        android:id='@+id/" + stringsEntryName + @"' />
");
            metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "DatePicker", name = stringsEntryName });
            return fieldXml.Replace("'", "\"");
        }

        string getXamlDefinitionForTextField(FieldDefinition field)
        {
            var stringsEntryText = field.DisplayLabel;
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"
    <EditText
        android:layout_width='match_parent'
        android:layout_height='wrap_content'
        android:id='@+id/" + stringsEntryName + @"'
        android:hint='@string/" + stringsEntryName + @"' />");
            metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "EditText", name = stringsEntryName });
            metaDataProvider.AddStringResource(stringsEntryName, stringsEntryText);
            return fieldXml.Replace("'", "\"");
        }

        string getXamlDefinitionForLabel(FieldDefinition field)
        {
            var stringsEntryText = field.DisplayLabel;
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"
    <TextView
android:text='@string/" + stringsEntryName + @"'
android:textColor='@android:color/holo_blue_dark'
android:textAppearance='?android:attr/textAppearanceSmall'
        android:layout_width='match_parent'
        android:layout_height='wrap_content'
        android:id='@+id/" + stringsEntryName + @"' />");
            //metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "TextView", name = stringsEntryName });
            metaDataProvider.AddStringResource(stringsEntryName, stringsEntryText);
            return fieldXml.Replace("'", "\"");
        }

        string getXamlLabelDefForEnumeratedFields(FieldDefinition field)
        {
            var stringsEntryText = field.DisplayLabel;
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"
    <TextView
android:text='@string/" + stringsEntryName + @"'
android:textColor='@android:color/holo_blue_dark'
android:textAppearance='?android:attr/textAppearanceSmall'
        android:minWidth='25dp'
        android:minHeight='25dp'
        android:layout_width='match_parent'
        android:layout_height='wrap_content'
        android:id='@+id/l_" + stringsEntryName + "' />");
            metaDataProvider.AddStringResource(stringsEntryName, stringsEntryText);
            return fieldXml.Replace("'", "\"");
        }

        string getXamlLabelDefForRadioGroup(FieldDefinition field)
        {
            var stringsEntryName = field.ViewName;
            var fieldXml = (@"<RadioGroup
android:orientation='horizontal'
        android:minWidth='25dp'
        android:minHeight='25dp'
        android:layout_width='match_parent'
        android:layout_height='wrap_content'
        android:id='@+id/rg_" + stringsEntryName + "' >");
            return fieldXml.Replace("'", "\"");
        }

        List<string> getXamlLabelDefForCheckBox(FieldDefinition field)
        {
            var fieldOptionDefinitions = new List<string>();
            foreach (var option in field.FieldOptions.Values)
            {
                var optionName = field.ViewName + "_" + option.Clean();
            var fieldXml = (@" <CheckBox
            android:layout_width='wrap_content'
            android:layout_height='wrap_content'
            android:checked='false'
             android:text='@string/" + optionName + @"'
             android:id='@+id/" + optionName + "' />"
             );
                metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "CheckBox", name = optionName });
                metaDataProvider.AddStringResource(optionName, option);

                fieldOptionDefinitions.Add(fieldXml.Replace("'", "\""));
            }
            return fieldOptionDefinitions;
        }

        List<string> getXamlLabelDefForRadioButton(FieldDefinition field)
        {
            //var fieldItem = new FieldItem() { dataType = "RadioGroup", name = optionName }
            var fieldOptionDefinitions = new List<string>();
            foreach (var option in field.FieldOptions.Values)
            {
                var optionName = field.ViewName + "_" + option.Clean();
                var fieldXml = (@"
<RadioButton
            android:layout_width='wrap_content'
            android:layout_height='wrap_content'
            android:checked='false'
             android:text='@string/" + optionName + @"'
             android:id='@+id/" + optionName + "' />"
                 );

                metaDataProvider.ModelItems.Add(new FieldItem() { dataType = "RadioButton", name = optionName });
                metaDataProvider.AddStringResource(optionName, option);

                fieldOptionDefinitions.Add(fieldXml.Replace("'", "\""));
            }
            return fieldOptionDefinitions;
        }
    }
}
