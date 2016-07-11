using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using System.Linq;
using JhpDataSystem.store;
using Android.Content;
using JhpDataSystem.Utilities;
using JhpDataSystem.projects.ppx;

namespace JhpDataSystem.projects.ppx
{
    [Activity(Label = "@string/ppx_activitylabel", Icon = "@drawable/jhpiego_logo")]
    public class PPXHomeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            showPPXHome();
        }

        public void StartActivity(Type activityType, Type resultActivity)
        {
            var returnTypeString = Newtonsoft.Json.JsonConvert.SerializeObject(resultActivity);
            var intent = new Intent(this, activityType);
            intent.PutExtra(Constants.KIND_PPX_NEXTVIEW, returnTypeString);
            intent.SetFlags(ActivityFlags.ClearTop);
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && data != null && data.HasExtra(Constants.BUNDLE_SELECTEDCLIENT))
            {
                var nextResultActivity = data.GetStringExtra(Constants.KIND_PPX_NEXTVIEW);
                var nextResultType = Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(nextResultActivity);

                var clientString = data.GetStringExtra(Constants.BUNDLE_SELECTEDCLIENT);

                var intent = new Intent(this, nextResultType);
                intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);

                StartActivityForResult(intent, 0);
            }
        }

        void showPPXHome()
        {
            SetContentView(Resource.Layout.PrepexHome);
            var closeButton = FindViewById<Button>(Resource.Id.buttonClose);
            closeButton.Click += (sender, e) => {
                //close activity
                StartActivity(typeof(LauncherActivity));
            };

            var buttonClientEvaluation = FindViewById<Button>(Resource.Id.buttonClientEvaluation);
            buttonClientEvaluation.Click += (sender, e) =>
            {
                //showAddNewView(true);
                StartActivity(typeof(PP_ClientEval1));
            };

            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Click += (sender, e) =>
            {
                StartActivity(typeof(ClientSelectionActivity), typeof(PP_Unscheduled1));
            };
            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Click += (sender, e) =>
            {
                StartActivity(typeof(ClientSelectionActivity), typeof(PP_DeviceRemoval1));
            };

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Click += (sender, e) => {
                StartActivity(typeof(ClientSelectionActivity), typeof(PP_PostRemovalVisit1));
            };

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                StartActivity(typeof(FilteredGridDisplayActivity));
            };

            //buttonClientsDueFor
            var buttonClientsToCall = FindViewById<Button>(Resource.Id.buttonClientsToCall);
            if (buttonClientsToCall != null)
            {
                //buttonClientsToCall.Click += (sender, e) =>
                //{
                //    getClientsToCall();
                //};
            }

            var buttonClientsToSms = FindViewById<Button>(Resource.Id.buttonClientsToSms);
            if (buttonClientsToSms != null)
            {
                //buttonClientsToSms.Click += (sender, e) =>{
                //new SmsSender()
                //{
                //    appContext = this,
                //    message = "Message from JHP",
                //    phoneNumber = "0977424090"
                //}.Send();
                // //getClientsToSms();
                //};
            }

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Click += (sender, e) => 
            {
                getPrepexSuppliesReport();
            };
            //we get the number of unsync'd records
            var unsyncdRecs = new CloudDb(Assets).GetRecordsToSync();
            var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            buttonServerSync.Text = string.Format("Save to Server. {0} unsaved", unsyncdRecs.Count);
            buttonServerSync.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Performing action requested", Android.Widget.ToastLength.Short).Show();
                await AppInstance.Instance.CloudDbInstance.EnsureServerSync();
                //var res = await new got(Assets).trainAriaStark();
                //var resString = res == null ? "RES IS NULL" : res.ToString();
                Toast.MakeText(this, "Sync completed", Android.Widget.ToastLength.Short).Show();
            };
        }

        List<PPDeviceSizes> getPPDeviceSizes()
        {
            var allClients = new ClientSummaryLoader().Get();
            var allSizes = new Dictionary<int, PPDeviceSizes>();
            foreach (var client in allClients)
            {
                PPDeviceSizes current = null;
                var dayId = client.PlacementDate.DayOfYear;
                if (!allSizes.TryGetValue(dayId, out current))
                {
                    current = new PPDeviceSizes(dayId);
                    allSizes[dayId] = current;
                }
                current.Add(client.DeviceSize);
            }
            var toReturn = new List<PPDeviceSizes>();
            toReturn.AddRange(allSizes.Values);
            return toReturn;
        }

        private void getPrepexSuppliesReport()
        {
            //StartActivity(typeof(FilteredGridDisplayActivity));
            //we get all clients
            var allSizes = getPPDeviceSizes();            
            var resList = new List<string>();
            resList.Add(PPDeviceSizes.getHeader());
            foreach (var dayUsage in allSizes)
            {
                resList.Add(dayUsage.toDisplay());
            }

            var asString = string.Join(System.Environment.NewLine, resList);

            //compute summary of device usage
            //show the results
            new EmailSender()
            {
                appContext = this,
                receipients = new List<string>() {
                        "makandok@gmail.com", "makandok@yahoo.com" },
                messageSubject = "PP Device Usage Summary",
                message = asString
            }.Send();
        }

        //private void bindDateDialogEventsForView(int viewId)
        //{
        //    //we get all the relevant fields for this view
        //    var viewFields = GetFieldsForView(viewId);

        //    //we find the date fields
        //    var dateFields = (from field in viewFields
        //                      where field.dataType == Constants.DATEPICKER
        //                      select field).ToList();
        //    var context = this;
        //    //Android.Content.Res.Resources res = context.Resources;
        //    //string recordTable = res.GetString(Resource.String.RecordsTable);
        //    foreach (var field in dateFields)
        //    {
        //        //we convert these into int Ids
        //        int resID = context.Resources.GetIdentifier(
        //            Constants.DATE_BUTTON_PREFIX + field.name, "id", context.PackageName);
        //        if (resID == 0)
        //            continue;

        //        var dateSelectButton = FindViewById<Button>(resID);
        //        if (dateSelectButton == null)
        //            continue;

        //        //create events for them and their accompanying text fields
        //        dateSelectButton.Click += (a, b) =>
        //        {
        //            var dateViewId = context.Resources.GetIdentifier(
        //                Constants.DATE_TEXT_PREFIX + field.name, "id", context.PackageName);
        //            var sisterView = FindViewById<EditText>(dateViewId);
        //            if (sisterView == null)
        //                return;
        //            var frag = DatePickerFragment.NewInstance((time) =>
        //            {
        //                sisterView.Text = time.ToLongDateString();
        //            });
        //            frag.Show(FragmentManager, DatePickerFragment.TAG);
        //        };
        //    }
        //}

        //private void getDataForView(int viewId)
        //{
        //    //we get all the relevant fields for this view
        //    var viewFields = GetFieldsForView(viewId);

        //    //we find the date fields
        //    var dataFields = (from field in viewFields
        //                      where field.dataType == Constants.DATEPICKER
        //                      || field.dataType == Constants.EDITTEXT
        //                      || field.dataType == Constants.CHECKBOX
        //                      || field.dataType == Constants.RADIOBUTTON
        //                      select field).ToList();
        //    var context = this;
        //    var valueFields = new List<FieldValuePair>();
        //    foreach (var field in dataFields)
        //    {
        //        var resultObject = new FieldValuePair() {Field = field, Value = string.Empty };
        //        switch (field.dataType)
        //        {
        //            case Constants.DATEPICKER:
        //                {
        //                    var view = field.GetDataView<EditText>(this);
        //                    if (string.IsNullOrWhiteSpace(view.Text))
        //                        continue;

        //                    resultObject.Value = view.Text;
        //                   break;
        //                }
        //            case Constants.EDITTEXT:
        //                {
        //                    var view = field.GetDataView<EditText>(this);
        //                    if (string.IsNullOrWhiteSpace(view.Text))
        //                        continue;

        //                    resultObject.Value = view.Text;
        //                    break;
        //                }
        //            case Constants.CHECKBOX:
        //                {
        //                    var view = field.GetDataView<CheckBox>(this);
        //                    if (!view.Checked)
        //                    {
        //                        continue;
        //                    }
        //                    resultObject.Value = Constants.DEFAULT_CHECKED;
        //                    break;
        //                }
        //            case Constants.RADIOBUTTON:
        //                {
        //                    var view = field.GetDataView<RadioButton>(this);
        //                    if (!view.Checked)
        //                    {
        //                        continue;
        //                    }
        //                    resultObject.Value = Constants.DEFAULT_CHECKED;
        //                    break;
        //                }
        //            default:
        //                {
        //                    throw new ArgumentNullException("Could not find view for field " + field.name);
        //                }
        //        }

        //        if (string.IsNullOrWhiteSpace(resultObject.Value))
        //        {
        //            throw new ArgumentNullException("Could not find view for field " + field.name);
        //        }
        //        valueFields.Add(resultObject);
        //    }

        //    AppInstance.Instance.TemporalViewData[viewId] = valueFields;
        //}

        //private List<FieldItem> GetFieldsForView(int viewId)
        //{
        //    var filterString = string.Empty;
        //    switch (viewId)
        //    {
        //        case Resource.Layout.prepexreg1:
        //            filterString = Constants.PP_VIEWS_1;
        //            break;
        //        case Resource.Layout.prepexreg2:
        //            filterString = Constants.PP_VIEWS_2;
        //            break;
        //        case Resource.Layout.prepexreg3:
        //            filterString = Constants.PP_VIEWS_3;
        //            break;
        //        case Resource.Layout.prepexreg4:
        //            filterString = Constants.PP_VIEWS_4;
        //            break;
        //    }
        //    var fields = (AppInstance.Instance.PPXFieldItems.Where(t => t.pageName == filterString)).ToList();
        //    return fields;
        //}

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
    }
}