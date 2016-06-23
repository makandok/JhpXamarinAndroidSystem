using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem.store;
using System.Globalization;

namespace JhpDataSystem
{
    [Activity(Label = "Client List")]
    public class GridDisplayActivity : ListActivity
    {
        List<PrepexClientSummary> _allPrepexClients;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ListView.FastScrollEnabled = true;
            var all = new LocalDB3().DB
                .Table<PrepexClientSummary>()                
                .OrderBy(t => t.PlacementDate)
                .ToList();
            all.ForEach(t => t.Id = new KindKey(t.KindKey));
            _allPrepexClients = all;
            var adapter = new ClientSummaryAdapter(this, all);
            ListAdapter = adapter;         
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }

        public class ClientSummaryAdapter : BaseAdapter<PrepexClientSummary>
        {
            List<PrepexClientSummary> _myList;
            ListActivity _context;
            public ClientSummaryAdapter(ListActivity context, List<PrepexClientSummary> clientList)
            {
                _context = context;
                _myList = clientList;
            }
            
            public override PrepexClientSummary this[int position]
            {
                get
                {
                    return _myList[position];
                }
            }

            public override int Count
            {
                get
                {
                    return _myList.Count;
                }
            }

            public override long GetItemId(int position)
            {
                return _myList[position].Id.Value.GetHashCode();
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var client = _myList[position];
                var myView = convertView ??
                     _context.LayoutInflater.Inflate(Resource.Layout.clientsummary, parent, false);
                //clientSummaryTDate
                var placementDate = client.PlacementDate;
                var daysElapsed = DateTime.Now.Subtract(placementDate).TotalDays;
                myView.FindViewById<TextView>(Resource.Id.clientSummaryTDate)
                    .Text = Convert.ToString("Day "+ Convert.ToInt32(daysElapsed) );
                myView.FindViewById<TextView>(Resource.Id.clientSummaryNames)
                    .Text = Convert.ToString(client.Names);
                myView.FindViewById<TextView>(Resource.Id.clientSummaryCardSerial)
                    .Text = "Card Id: " + Convert.ToString(client.FormSerial);
                myView.FindViewById<TextView>(Resource.Id.clientSummaryMCNumber)
                    .Text = "MC #: " + Convert.ToString(client.ClientNumber);
                myView.FindViewById<TextView>(Resource.Id.clientSummaryPlacementDate)
                    .Text = client.PlacementDate.ToString("d MMM, yyyy", CultureInfo.InvariantCulture);
                myView.FindViewById<TextView>(Resource.Id.clientSummaryTelephone)
                    .Text = Convert.ToString(client.ContactPhone);
                return myView;
            }
        }
    }
}