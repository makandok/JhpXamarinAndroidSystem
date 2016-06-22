using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;

namespace JhpDataSystem.store
{
    public class LookupProvider
    {
        public LookupProvider()
        {
            Lookups = new List<NameValuePair>();
        }

        public KindName kind { get; set; }

        public List<NameValuePair> Lookups { get; set; }

        internal void Add(string name, string value)
        {
            Lookups.Add(new NameValuePair() { Name = name, Value = value });
        }

        //internal void Save()
        //{

        //}
    }
}