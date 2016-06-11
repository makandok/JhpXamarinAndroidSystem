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
using Android.Content.Res;
using System.Json;
using System.Threading.Tasks;
using JhpDataSystem.db;
using JhpDataSystem.store;
using JhpDataSystem.model;
using JhpDataSystem.Security;
using Newtonsoft.Json;
using JhpDataSystem.Modules.Prepex;


namespace JhpDataSystem.Modules
{
    [Activity(Label = "BrowserDataEntryActivity")]
    public class BrowserDataEntryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application 
            var fields = Assets.Open("");
            SetContentView(Resource.Layout.DataEntryPage);
        }
    }
}