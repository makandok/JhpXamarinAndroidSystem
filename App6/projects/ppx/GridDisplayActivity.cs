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
using JhpDataSystem.Utilities;
using System.Threading.Tasks;
using Android.Content;

namespace JhpDataSystem.projects.ppx
{
    [Activity(Label = "Select Record")]
    public class SelectRecordsActivity : Activity, ListView.IOnItemClickListener
    {
        List<DisplayRecordSummary> _allItem;
        RecordSummary _selectedItem = null;

        public string DisplayName { get; set; }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            //we get the selected client and return
            _selectedItem = _allItem[position].Wrapped;
        }

        List<DisplayRecordSummary> getRecordsForClient(string entityId)
        {
            //we get entityid from the intent
            var res = new LocalDB3().DB.Query<RecordSummary>(
               string.Format("select * from {0} where EntityId = @entityid",
               Constants.SYS_KIND_RECORDSUMMARY), entityId);
            return (from table in res select new DisplayRecordSummary(table)).ToList();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (this.Intent.Extras == null)
                return;

            SetContentView(Resource.Layout.clientlist);

            var clientString = this.Intent.Extras
    .GetString(Constants.BUNDLE_SELECTEDCLIENT);
            var client = Newtonsoft.Json.JsonConvert
                .DeserializeObject<PPClientSummary>(clientString);
            var entityId = client.EntityId.Value;

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;
            listview.OnItemClickListener = this;

            //LocalEntityStore.Instance.updateRecordSummaryTable();

            var recordSummaries = getRecordsForClient(entityId);

            _allItem = recordSummaries;
            var adapter = new RecordSummaryAdapter(this, listview, recordSummaries);
            listview.Adapter = adapter;

            //we hide the client summary options
            var rgroupCSOptions = FindViewById<RadioGroup>(Resource.Id.rgroupCSOptions);
            rgroupCSOptions.Visibility = ViewStates.Gone;

            var buttonPerformAction = FindViewById<Button>(Resource.Id.buttonPerformAction);
            buttonPerformAction.Text = "Edit Selected Record";
            buttonPerformAction.Click += performActionSpecified;
        }

        void performActionSpecified(object sender, EventArgs e)
        {
            if (_selectedItem == null)
            {
                Android.Widget.Toast.MakeText(this,
                    "No clients selected. Please select a client.",
                    Android.Widget.ToastLength.Long).Show();
                return;
            }

            var intent = new Intent()
                .PutExtra(Constants.BUNDLE_SELECTEDCLIENT,
                    this.Intent.Extras.GetString(Constants.BUNDLE_SELECTEDCLIENT))
                .PutExtra(Constants.BUNDLE_SELECTEDRECORD_ID, _selectedItem.Id)
                .PutExtra(Constants.BUNDLE_SELECTEDRECORD,
                Newtonsoft.Json.JsonConvert.SerializeObject(_selectedItem));
            SetResult(Result.Ok, intent);
            Finish();
        }
    }

    public class RecordSummaryAdapter : BaseAdapter<DisplayRecordSummary>
    {
        List<DisplayRecordSummary> _myList;
        Activity _context;
        public RecordSummaryAdapter(Activity context, ListView listview, List<DisplayRecordSummary> clientRecords)
        {
            _context = context;            
            _myList = clientRecords;
        }

        public override DisplayRecordSummary this[int position]
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
            return _myList[position].Wrapped.getItemId();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var record = _myList[position];
            var myView = convertView ??
                 _context.LayoutInflater.Inflate(Resource.Layout.recordsummaryview, parent, false);
            myView.FindViewById<TextView>(Resource.Id.recordVisitType)
                .Text = string.Format(
                "{0} - {1}", record.DisplayKindName, record.Wrapped.VisitDate.ToShortDateString()
                );
            return myView;
        }
    }

    [Activity(Label = "Client Selector")]
    public class ClientSelectionActivity : Activity, ListView.IOnItemClickListener
    {
        ClientSummaryAdapter _defaultAdapter = null;
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
            
            if (this.Intent.Extras!=null && this.Intent.Extras.ContainsKey(Constants.KIND_PPX_NEXTVIEW))
            {
                //KIND_PP_NEXTVIEW
                NEXT_TYPE = this.Intent.Extras.GetString(Constants.KIND_PPX_NEXTVIEW);
            }

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new PpxLookupProvider().Get();
            _defaultAdapter = new ClientSummaryAdapter(this, listview, _allPrepexClients);

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

    [Activity(Label = "Client List")]
    public class FilteredGridDisplayActivity : Activity, ListView.IOnItemClickListener
    {
        ClientSummaryAdapter _defaultAdapter = null;
        List<PPClientSummary> _allPrepexClients;
        PPClientSummary _selectedClient = null;
        List<int> _listOptions = null;
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _selectedClient = _allPrepexClients[position];
            Android.Widget.Toast.MakeText(this, _selectedClient.Names, Android.Widget.ToastLength.Short).Show();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientlist);

            var listview = FindViewById<ListView>(Resource.Id.listviewClientList);
            listview.FastScrollEnabled = true;
            listview.FastScrollAlwaysVisible = true;

            listview.OnItemClickListener = this;

            _allPrepexClients = new PpxLookupProvider().Get();
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

        //void startClientRecordEdit(object sender, EventArgs e)
        //{
        //    //get selected client
        //    if (_selectedClient == null)
        //    {
        //        Toast.MakeText(this, Resource.String.ppx_noclientselected, Android.Widget.ToastLength.Long).Show();
        //        return;
        //    }
        //    //get list of forms for this client
        //    //allow user to pick the form to edit
        //    //show editor
        //}

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

        void showSendSmsDialog(List<PPClientSummary> clients)
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
        async Task<bool> updateDay6DateAndSave(List<PPClientSummary> clients, DateTime dateSmsd)
        {
            clients.ForEach(t => t.Day6SmsReminderDate = dateSmsd);
            new LocalDB3().DB.UpdateAll(clients);
            return true;
        }

        async Task<bool> sendSms(List<PPClientSummary> clients)
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

        List<PPClientSummary> getClients4TMinus(int daysPast)
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

    public class ClientSummaryAdapter : BaseAdapter<PPClientSummary>
    {
       List<PPClientSummary> _myList;
        Activity _context;
        public int tMinus { get; set; }
        public ClientSummaryAdapter(Activity context, ListView listview, List<PPClientSummary> clientList)
        {
            tMinus = -1;
            _context = context;
            _myList = clientList;
        }

        public override PPClientSummary this[int position]
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