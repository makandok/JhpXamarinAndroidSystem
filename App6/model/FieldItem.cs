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

namespace JhpDataSystem.model
{
    public class FieldItem
    {
        //public int fieldId { get; set; }
        public string name { get; set; }
        public string dataType { get; set; }
        public string pageName { get; set; }
        //public List<string> options { get; set; }
    }
}