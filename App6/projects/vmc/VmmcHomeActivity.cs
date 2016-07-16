using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem.store;
using Android.Content;
using JhpDataSystem.Utilities;
using System.Linq;
using System.Threading.Tasks;
using JhpDataSystem.projects.vmc.activity;

namespace JhpDataSystem.projects.vmc
{
    [Activity(Label = "@string/vmc_activitylabel", Icon = "@drawable/jhpiego_logo")]
    public class VmmcHomeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            showPPXHome();
        }

        PPClientSummary getClientFromIntent(Intent data)
        {
            var clientString = data.GetStringExtra(Constants.BUNDLE_SELECTEDCLIENT);
            return Newtonsoft.Json.JsonConvert
                .DeserializeObject<PPClientSummary>(clientString);
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
            if (resultCode != Result.Ok || data == null)
                return;

            if (data.HasExtra(Constants.BUNDLE_SELECTEDRECORD))
            {
                //result is from RecordSelector
                //we get the selected record id and client
                var recSummaryStr = data.Extras.GetString(Constants.BUNDLE_SELECTEDRECORD);
                var recSumm = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<RecordSummary>(recSummaryStr);

                //selectedRecordSummary
                var clientString = data.Extras.GetString(Constants.BUNDLE_SELECTEDCLIENT);

                //we load the record
                var jsonRecord = new TableStore(recSumm.KindName).Get(new KindKey(recSumm.Id)).FirstOrDefault();
                if (jsonRecord == null)
                    return;

                var kindActivityType = getActivityForKind(recSumm.KindName);
                var intent = new Intent(this, kindActivityType);
                intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);
                intent.PutExtra(Constants.BUNDLE_DATATOEDIT, jsonRecord.Value);
                //var dataset = new PPDataSet().fromJson(jsonRecord);
                intent.SetFlags(ActivityFlags.ClearTop);
                StartActivityForResult(intent, 0);
            }
            else if (data.HasExtra(Constants.BUNDLE_SELECTEDCLIENT))
            {
                //result is from client selector
                var nextResultActivity = data.GetStringExtra(Constants.KIND_PPX_NEXTVIEW);
                var nextResultType = Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(nextResultActivity);

                var clientString = data.GetStringExtra(Constants.BUNDLE_SELECTEDCLIENT);

                var intent = new Intent(this, nextResultType);
                intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);

                StartActivityForResult(intent, 0);
            }
        }

        private Type getActivityForKind(string viewName)
        {
            return typeof(VmmcPostOp1);
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
                StartActivity(typeof(VmmcRegAndProc1));
            };
            buttonClientEvaluation.Text = "Registration and Procedure";


            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Visibility = Android.Views.ViewStates.Gone;
            //buttonUnscheduled.Click += (sender, e) =>
            //{
            //    StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_Unscheduled1));
            //};
            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Click += (sender, e) =>
            {
                StartActivity(typeof(VmmcClientSelectionActivity), typeof(VmmcPostOp1));
            };
            buttonDeviceRemovalVisit.Text = "Post Operation";

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Visibility = Android.Views.ViewStates.Gone;
            //buttonPostRemovalVisit.Click += (sender, e) => {
            //    StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_PostRemovalVisit1));
            //};

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                StartActivity(typeof(VmmcFilteredGridDisplayActivity));
            };

            //buttonViewList
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) => {
                StartActivity(typeof(VmmcClientSelectionActivity), typeof(VmmcRecordSelectorActivity));
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

            var buttonViewRecordSummaries = FindViewById<Button>(Resource.Id.buttonViewRecordSummaries);
            buttonViewRecordSummaries.Click += async (sender, e) =>
            {
                await getClientSummaryReport();
            };

            var buttonSendReport = FindViewById<Button>(Resource.Id.buttonSendReport);
            buttonSendReport.Click += (sender, e) =>
            {
                var textRecordSummaries = FindViewById<TextView>(Resource.Id.textRecordSummaries);
                if (textRecordSummaries == null)
                    return;

                if (string.IsNullOrWhiteSpace(textRecordSummaries.Text) ||
                            textRecordSummaries.Text ==
                            Resources.GetString(Resource.String.sys_not_updated))
                {
                    sendToast("No data to send. Run a report first", ToastLength.Long);
                    return;
                }

                new EmailSender()
                {
                    appContext = this,
                    receipients = new List<string>() {
                            "makando.kabila@jhpiego.org"},
                    messageSubject = "Summary Report",
                    message = textRecordSummaries.Text
                }.Send();
            };

            //we get the number of unsync'd records
            var unsyncdRecs = new CloudDb(Assets).GetRecordsToSync();
            var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            buttonServerSync.Text = string.Format("Save to Server. {0} unsaved", unsyncdRecs.Count);
            buttonServerSync.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Performing action requested", Android.Widget.ToastLength.Short).Show();
                var res2 = await AppInstance.Instance.CloudDbInstance.EnsureServerSync(sendToast);
                //var res = await new got(Assets).trainAriaStark();
                //var resString = res == null ? "RES IS NULL" : res.ToString();
                Toast.MakeText(this, "Sync completed", Android.Widget.ToastLength.Short).Show();
            };
        }

        void sendToast(string message, ToastLength length)
        {
            Toast.MakeText(this, message, length).Show();
        }

        List<PPDeviceSizes> getPPDeviceSizes()
        {
            var allClients = new VmmcLookupProvider().Get();
            var allSizes = new Dictionary<int, PPDeviceSizes>();
            foreach (var client in allClients)
            {
                PPDeviceSizes current = null;
                var dayId = (client.PlacementDate.Year * 1000) + client.PlacementDate.DayOfYear;
                if (!allSizes.TryGetValue(dayId, out current))
                {
                    current = new PPDeviceSizes(client.PlacementDate);
                    allSizes[dayId] = current;
                }
                current.Add(client.DeviceSize);
            }

            var toReturn = new List<PPDeviceSizes>();
            toReturn.AddRange(allSizes.Values);
            return toReturn;
        }

        private async Task<int> getClientSummaryReport()
        {
            var countRes = LocalEntityStore.Instance.GetAllBobsCount();

            var asList = (from item in countRes
                          let displayItem = new NameValuePair()
                          {
                              Name = Constants.PPX_KIND_DISPLAYNAMES[item.Name],
                              Value = item.Value
                          }
                          select displayItem.toDisplayText()).ToList();
            var resList = new List<string>() {
                Resources.GetString(Resource.String.sys_summary_blobcount),
                NameValuePair.getHeaderText() };
            resList.AddRange(asList);

            //also add client summary
            var recCount = new VmmcLookupProvider().GetCount();
            var clientSummaryCount = new NameValuePair()
            {
                Name = "ppx Client Summary",
                Value = recCount.ToString()
            };
            resList.Add(clientSummaryCount.toDisplayText());

            var summaryInfo = new LocalDB3().DB.Query<NameValuePair>(
                                string.Format(
                "select KindName as Name, count(*) as Value from {0} group by KindName",
                Constants.SYS_KIND_RECORDSUMMARY)
                );
            resList.Add(System.Environment.NewLine);
            resList.Add("Summary of Records in "+ Constants.SYS_KIND_RECORDSUMMARY);
            var asStringList = (from nvp in summaryInfo
                                select nvp.toDisplayText()).ToList();
            resList.AddRange(asStringList);
            var asText = string.Join(System.Environment.NewLine, resList);
            setTextResults(asText);
            return 0;
        }

        private void getPrepexSuppliesReport()
        {
            //we get all clients
            var allSizes = getPPDeviceSizes();            
            var resList = new List<string>();
            resList.Add(Resources.GetString(Resource.String.ppx_sys_deviceusage));
            resList.Add(System.Environment.NewLine);
            resList.Add(PPDeviceSizes.getHeader());
            foreach (var dayUsage in allSizes)
            {
                resList.Add(dayUsage.toDisplay());
            }

            var asString = string.Join(System.Environment.NewLine, resList);
            setTextResults(asString);
        }

        void setTextResults(string asString)
        {
            var textRecordSummaries = FindViewById<TextView>(Resource.Id.textRecordSummaries);
            if (textRecordSummaries != null)
            {
                textRecordSummaries.Text = asString;
            }
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