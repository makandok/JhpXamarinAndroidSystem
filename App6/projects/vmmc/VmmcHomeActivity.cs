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

namespace JhpDataSystem.projects.vmmc
{
    [Activity(Label = "Vmmc Home", Icon = "@drawable/jhpiego_logo")]
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
            return null; // typeof(PP_DeviceRemoval1);
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
            buttonClientEvaluation.Text = "Registration and Proc";
            buttonClientEvaluation.Click += (sender, e) =>
            {
                StartActivity(typeof(VmmcRegAndProc1));
            };

            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Text = "Post Operation";
            buttonUnscheduled.Click += (sender, e) =>
            {
                StartActivity(typeof(ClientSelectionActivity), typeof(VmmcPostOp1));
            };

            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Visibility = Android.Views.ViewStates.Invisible;
            //buttonDeviceRemovalVisit.Click += (sender, e) =>
            //{
            //    StartActivity(typeof(ClientSelectionActivity), typeof(PP_DeviceRemoval1));
            //};

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Visibility = Android.Views.ViewStates.Invisible;
            //buttonPostRemovalVisit.Click += (sender, e) => {
            //    StartActivity(typeof(ClientSelectionActivity), typeof(PP_PostRemovalVisit1));
            //};

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                StartActivity(typeof(FilteredGridDisplayActivity));
            };

            //buttonViewList
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) => {
                StartActivity(typeof(ClientSelectionActivity), typeof(SelectRecordsActivity));
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Visibility = Android.Views.ViewStates.Invisible;
            //buttonSupplies.Click += (sender, e) => 
            //{
            //    getSuppliesReport();
            //};

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

        private void getSuppliesReport()
        {
            ////StartActivity(typeof(FilteredGridDisplayActivity));
            ////we get all clients
            //var allSizes = getPPDeviceSizes();
            //var resList = new List<string>();
            //resList.Add(PPDeviceSizes.getHeader());
            //foreach (var dayUsage in allSizes)
            //{
            //    resList.Add(dayUsage.toDisplay());
            //}

            //var asString = string.Join(System.Environment.NewLine, resList);

            ////compute summary of device usage
            ////show the results
            //new EmailSender()
            //{
            //    appContext = this,
            //    receipients = new List<string>() {
            //            "makandok@gmail.com", "makandok@yahoo.com" },
            //    messageSubject = "PP Device Usage Summary",
            //    message = asString
            //}.Send();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
    }
}