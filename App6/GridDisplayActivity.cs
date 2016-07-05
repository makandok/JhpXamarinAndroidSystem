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
using Android.Support.Design.Widget;

namespace JhpDataSystem
{
    public class ClientSummaryLoader
    {
        public List<PrepexClientSummary>  Get()
        {
            var all = new LocalDB3().DB
                .Table<PrepexClientSummary>()
                .OrderBy(t => t.PlacementDate)
                .ToList();
            all.ForEach(t => t.Id = new KindKey(t.KindKey));
            return all;
        }
    }

    //[Activity(Label = "Client List")]
    //public class GridDisplayActivity : ListActivity
    //{
    //    List<PrepexClientSummary> _allPrepexClients;
    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState);
    //        ListView.FastScrollEnabled = true;
    //        _allPrepexClients = new ClientSummaryLoader().Get();
    //        var adapter = new ClientSummaryAdapter(this, ListView, _allPrepexClients);
    //        ListAdapter = adapter;
    //    }

    //    protected override void OnListItemClick(ListView l, View v, int position, long id)
    //    {
    //        var t = _allPrepexClients[position];
    //        Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
    //    }
    //}

    [Activity(Label = "Client List")]
    public class FilteredGridDisplayActivity : Activity, ListView.IOnItemClickListener
    {
        ClientSummaryAdapter _defaultAdapter = null;
        List<PrepexClientSummary> _allPrepexClients;
        Dictionary<int, EventHandler> _behaviours;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var t = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new ClientSummaryLoader().Get();
            _defaultAdapter = new ClientSummaryAdapter(this, listview, _allPrepexClients);

            listview.Adapter = _defaultAdapter;

            //we bind the events for the action buttons on top
            _behaviours = new Dictionary<int, EventHandler>()
            {
                     {Resource.Id.buttonCSummmCall3, day3Call},
                     {Resource.Id.buttonCSummSms6, day6Sms},

                     {Resource.Id.buttonCSummSmsOrCall7, day7NoShowCall},
                     {Resource.Id.buttonCSummCall14, day14Call},

                     {Resource.Id.buttonCSummCall49, day49Call},
                     {Resource.Id.buttonCSummCall56, day56Call},
            };
            foreach (var behaviour in _behaviours)
            {
                var btn = FindViewById<Button>(behaviour.Key);
                btn.Click += behaviour.Value;
            }
        }

        void day3Call(object sender, EventArgs e)
        {
            //todo: complete day3call
            showClients4TMinus(3);
        }

        void showClients4TMinus(int daysPast)
        {
            //textCSummOptionsLabel   
            var textCSummOptionsLabel = FindViewById<TextView>(Resource.Id.textCSummOptionsLabel);
            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);

            //if someone clicks the button again, we show the full list
            var currentAdapter = listview.Adapter as ClientSummaryAdapter;
            if(currentAdapter!=null && currentAdapter.tMinus == daysPast)
            {
                if (textCSummOptionsLabel != null)
                    textCSummOptionsLabel.Text = "All Clients";

                //Android.Widget.Toast.MakeText(this, "Showing all clients", Android.Widget.ToastLength.Short).Show();
                listview.Adapter = _defaultAdapter;
                return;
            }

            //else we filter based on choice
            var tMinus = DateTime.Now.Subtract(new TimeSpan(daysPast, 0, 0, 0));

            if (textCSummOptionsLabel != null)
                textCSummOptionsLabel.Text = "Clients for " + tMinus.ToShortDateString();

            //Android.Widget.Toast.MakeText(this, "Showing clients for " + tMinus.ToShortDateString(), Android.Widget.ToastLength.Short).Show();
            listview.Adapter = new ClientSummaryAdapter(this,
                listview
                , getClients4TMinus(daysPast)
                )
            { tMinus = daysPast };

            //we show the fab
            //var fab = new FloatingActionButton(this, );
        }

        List<PrepexClientSummary> getClients4TMinus(int daysPast)
        {
            var tMinus = DateTime.Now.Subtract(new TimeSpan(daysPast, 0, 0, 0));
            return _allPrepexClients.Where(t =>
                t.PlacementDate.Day == tMinus.Day &&
                t.PlacementDate.Month == tMinus.Month &&
                t.PlacementDate.Year == tMinus.Year
                ).ToList();
        }

        void day6Sms(object sender, EventArgs e)
        {
            showClients4TMinus(6);
        }

        void day7NoShowCall(object sender, EventArgs e)
        {
            showClients4TMinus(7);
        }

        void day14Call(object sender, EventArgs e)
        {
            showClients4TMinus(14);
        }

        void day49Call(object sender, EventArgs e)
        {
            showClients4TMinus(49);
        }

        void day56Call(object sender, EventArgs e)
        {
            showClients4TMinus(56);
        }

        void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }        
    }

    public class ClientSummaryAdapter : BaseAdapter<PrepexClientSummary>
    {
       List<PrepexClientSummary> _myList;
        Activity _context;
        public int tMinus { get; set; }
        public ClientSummaryAdapter(Activity context, ListView listview, List<PrepexClientSummary> clientList)
        {
            tMinus = -1;
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
            return _myList[position].getItemId();
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
                .Text = Convert.ToString("Day " + Convert.ToInt32(daysElapsed));
            myView.FindViewById<TextView>(Resource.Id.clientSummaryNames)
                .Text = Convert.ToString(client.Names);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryCardSerial)
                .Text = "Card Id: " + Convert.ToString(client.FormSerial);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryMCNumber)
                .Text = "MC #: " + Convert.ToString(client.ClientNumber);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryPlacementDate)
                .Text = client.PlacementDate.ToString("d MMM, yyyy", CultureInfo.InvariantCulture);
            myView.FindViewById<TextView>(Resource.Id.clientSummaryTelephone)
                .Text = Convert.ToString(client.Telephone);
            return myView;
        }
    }
}