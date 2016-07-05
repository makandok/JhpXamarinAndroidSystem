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
using JhpDataSystem.Utilities;
using System.Threading.Tasks;

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

        public int Update(List<PrepexClientSummary> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
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
        //Dictionary<int, EventHandler> _behaviours;
        List<int> _listOptions = null;
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
            _listOptions = new List<int>()
            {
                Resource.Id.buttonCSummmCall3,Resource.Id.buttonCSummSms6,Resource.Id.buttonCSummSmsOrCall7,
                Resource.Id.buttonCSummCall14,Resource.Id.buttonCSummCall49,Resource.Id.buttonCSummCall56
            };

            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroup.CheckedChange += applyFilter;

            //buttonPerformAction
            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Click += performActionSpecified;
        }

        void performActionSpecified(object sender, EventArgs e)
        {
            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            if (rgroup != null)
            {
                //we get the checked item
                var checkedButtonId = rgroup.CheckedRadioButtonId;
                switch (checkedButtonId)
                {
                    //case Resource.Id.buttonCSummmAll:
                    case Resource.Id.buttonCSummSms6:
                        //we send an SMS after confirmation
                        var clients = getClients4TMinus(6);
                        if (clients.Count == 0)
                        {
                            Toast.MakeText(this, Resources.GetString(Resource.String.confirm_action),
                                ToastLength.Long).Show();
                            return;
                        }

                        //get check if we havent already sent an SMS
                        //we use the placement date to track this cohort
                        //if so, we alert and return or ask user to still send
                        var alreadySmsd = clients.Any(t => t.Day6SmsReminderDate != DateTime.MinValue);
                        if (alreadySmsd)
                        {
                            new AlertDialog.Builder(this)
.SetTitle(Resources.GetString(Resource.String.confirm_action))
.SetMessage(string.Format(
Resources.GetString(Resource.String.sms_clientsalreadysmsd),
clients.Count, clients.Count == 1 ? "" : "s"))
.SetPositiveButton("OK", async (senderAlert, args) =>
{
    showSendSmsDialog(clients);
})
.SetNegativeButton("Cancel", (senderAlert, args) => { })
.Create()
.Show();
                        }
                        else
                        {
                            //we prompt
                            showSendSmsDialog(clients);
                        }

                        break;

                    case Resource.Id.buttonCSummmCall3:
                    case Resource.Id.buttonCSummSmsOrCall7:
                    case Resource.Id.buttonCSummCall14:
                    case Resource.Id.buttonCSummCall49:
                    case Resource.Id.buttonCSummCall56:
                        Toast.MakeText(this,
                            Resources.GetString(Resource.String.remembertocall),
                            //"Please remember to call the clients indicated here", 
                            ToastLength.Long).Show();
                        break;
                    default:
                        //do nothing
                        Toast.MakeText(this, "Nothing to do", ToastLength.Long).Show();
                        break;
                }
            }
        }

        void showSendSmsDialog(List<PrepexClientSummary> clients)
        {
            new AlertDialog.Builder(this)
.SetTitle(Resources.GetString(Resource.String.confirm_action))
.SetMessage(string.Format(
Resources.GetString(Resource.String.sms_confirmation),
clients.Count, clients.Count == 1 ? "" : "s"))
.SetPositiveButton("OK", async (senderAlert, args) => {
await sendSms(clients);
await updateDay6DateAndSave(clients, DateTime.Now);
})
.SetNegativeButton("Cancel", (senderAlert, args) => { })
.Create()
.Show();
        }

        //saveClientSummary
        async Task<bool> updateDay6DateAndSave(List<PrepexClientSummary> clients, DateTime dateSmsd)
        {
            clients.ForEach(t => t.Day6SmsReminderDate = dateSmsd);
            new LocalDB3().DB.UpdateAll(clients);
            return true;
        }

        async Task<bool> sendSms(List<PrepexClientSummary> clients)
        {
            //we send           
            Toast.MakeText(this, "Started sending messages", ToastLength.Short).Show();
            var bulkSender = new BulkSmsSender()
            {
                contactNumbers = clients,
                CurrentContext = this,
                formattedText = Resources.GetString(Resource.String.sms_msgwithname)
            };
            await bulkSender.Send();
            Toast.MakeText(this, "Completed sending messages", ToastLength.Short).Show();

            //save to db list of clients send

            return true;
        }

        void applyFilter(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            //rgroupCSOptions
            var rgroup = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            if (rgroup != null)
            {
                //we get the checked item
                var checkedButtonId = rgroup.CheckedRadioButtonId;
                switch (checkedButtonId)
                {
                    case Resource.Id.buttonCSummmCall3:
                        day3Call();break;
                    case Resource.Id.buttonCSummSms6:
                        day6Sms(); break;
                    case Resource.Id.buttonCSummSmsOrCall7:
                        day7NoShowCall(); break;
                    case Resource.Id.buttonCSummCall14:
                        day14Call(); break;
                    case Resource.Id.buttonCSummCall49:
                        day49Call(); break;
                    case Resource.Id.buttonCSummCall56:
                        day56Call(); break;
                    default:
                        //buttonCSummmAll
                        showClients4TMinus(-1);
                        break;
                }
            }
        }

        void day3Call()
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
            if (daysPast == -1)
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

        void day6Sms()
        {
            showClients4TMinus(6);
        }

        void day7NoShowCall()
        {
            showClients4TMinus(7);
        }

        void day14Call()
        {
            showClients4TMinus(14);
        }

        void day49Call()
        {
            showClients4TMinus(49);
        }

        void day56Call()
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
                .Text = Convert.ToString("Day " + Math.Floor(daysElapsed));
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