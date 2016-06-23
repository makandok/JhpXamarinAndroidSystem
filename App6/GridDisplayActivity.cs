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
using JhpDataSystem.store;

namespace JhpDataSystem
{
    [Activity(Label = "GridDisplayActivity")]
    public class GridDisplayActivity : ListActivity
    {
        List<PrepexClientSummary> clients;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //SetContentView(Resource.Layout.griddisplay);
            //var gridview = FindViewById<GridView>(Resource.Id.gridview);
            var all = new LocalDB3().DB
                .Table<PrepexClientSummary>()
                .OrderBy(t => t.PlacementDate)
                .ToList();
            clients = all;

            //List<object> sArray = new List<object>{ "This", "is", 3.5, true, 2, "for", "bla" };
            //var adp = new ArrayAdapter(this, Resource.Layout.griddisplay, all);
            ListAdapter = new ArrayAdapter<PrepexClientSummary>(this, 
                Android.Resource.Layout.SimpleListItem1, all);
            //gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
            //    Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            //};
        }
    }
}