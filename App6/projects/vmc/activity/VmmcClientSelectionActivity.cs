using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using Android.Content;

namespace JhpDataSystem.projects.vmc.activity
{
    [Activity(Label = "Client Selector")]
    public class VmmcClientSelectionActivity : Activity, ListView.IOnItemClickListener
    {
        VmmcClientSummaryAdapter _defaultAdapter = null;
        List<PPClientSummary> _allPrepexClients;
        PPClientSummary _selectedClient = null;
        string NEXT_TYPE = string.Empty;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //we get the selected client and return
            var t = _allPrepexClients[position];
            _selectedClient = t;
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            if (this.Intent.Extras != null && this.Intent.Extras.ContainsKey(Constants.KIND_PPX_NEXTVIEW))
            {
                //KIND_PP_NEXTVIEW
                NEXT_TYPE = this.Intent.Extras.GetString(Constants.KIND_PPX_NEXTVIEW);
            }

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new VmmcLookupProvider().Get();
            _defaultAdapter = new VmmcClientSummaryAdapter(this, listview, _allPrepexClients);

            listview.Adapter = _defaultAdapter;

            //we hide the client summary options
            var rgroupCSOptions = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroupCSOptions.Visibility = ViewStates.Gone;

            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Text = "Use Selected Client";
            buttonPerformAction.Click += performActionSpecified;
        }

        void performActionSpecified(object sender, EventArgs e)
        {
            if (_selectedClient == null)
            {
                Android.Widget.Toast.MakeText(this,
                    "No clients selected. Please select a client.",
                    Android.Widget.ToastLength.Long).Show();
                return;
            }

            var asString = Newtonsoft.Json.JsonConvert.SerializeObject(_selectedClient);
            var intent = new Intent().PutExtra(Constants.BUNDLE_SELECTEDCLIENT, asString);
            intent.PutExtra(Constants.KIND_PPX_NEXTVIEW, NEXT_TYPE);
            //intent.SetFlags(ActivityFlags.NewTask);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }

}