using System;
using System.Collections.Generic;
using JhpDataSystem.model;
using System.Linq;

namespace JhpDataSystem.projects.vmc
{
    //public class VmmcFormsBase : DataFormsBase<VmmcClientSummary>
    //{
    //    protected override Type getHomeActivityType()
    //    {
    //        return typeof(VmmcHomeActivity);
    //    }

    //    protected override List<NameValuePair> getModuleClientSummaries(IEnumerable<NameValuePair> data)
    //    {
    //        return new List<NameValuePair>();
    //    }

    //    protected override List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
    //    {
    //        var indexFieldNames = Constants.VMMC_IndexedFieldNames;
    //        var asList = string.Join(Environment.NewLine, indexFieldNames);
    //        var asList2 = string.Join(System.Environment.NewLine, (from t in data select t.Name));
            
    //        var toReturn = new List<NameValuePair>();
    //        foreach (var field in data)
    //        {
    //            if (indexFieldNames.Contains(field.Name))
    //            {
    //                toReturn.Add(field);
    //            }
    //        }
    //        return toReturn;
    //    }

    //    protected override List<FieldItem> GetFieldsForView(int viewId)
    //    {
    //        return AppInstance.Instance.VmmcFieldItems.Where(t => t.PageId == viewId).ToList();
    //    }
    //}
    public class VmmcFormsBase : DataFormsBase<VmmcClientSummary>
    {
        protected override Type getHomeActivityType()
        {
            return typeof(VmmcHomeActivity);
        }

        protected override List<NameValuePair> getModuleClientSummaries(IEnumerable<NameValuePair> data)
        {
            return new List<NameValuePair>();
        }

        protected override List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
        {
            var indexFieldNames = Constants.VMMC_IndexedFieldNames;
            return (data.Where(
                t => indexFieldNames.Contains(t.Name))).ToList();
        }

        protected override List<FieldItem> GetFieldsForView(int viewId)
        {
            return AppInstance.Instance.VmmcFieldItems.Where(t => t.PageId == viewId).ToList();
        }
    }
}