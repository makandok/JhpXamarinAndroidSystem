using System.Collections.Generic;
using Android.App;
using Android.Widget;
using JhpDataSystem.projects.vmc.activity;
using JhpDataSystem.projects.vmc.workflow;

namespace JhpDataSystem.projects.vmc
{
    [Activity(Label = "@string/vmc_activitylabel", Icon = "@drawable/jhpiego_logo")]
    public class VmmcHomeActivity : BaseHomeActivity<VmmcClientSummary>
    {
        protected override void showDefaultHome()
        {
            showPPXHome();
        }

        protected override List<BaseWorkflowController> getActivityWFControllers()
        {
            return new List<BaseWorkflowController>()
                                {
                                    new VmmcRegAndProcControl(),
                                    new VmmcPostOperationControl(),
                            };
        }

        void showPPXHome()
        {
            AppInstance.Instance.TemporalViewData.Clear();
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
                //StartActivity(typeof(VmmcRecordSelectorActivity));
                StartActivity(typeof(VmmcFilteredGridDisplayActivity));
            };

            //buttonEditRecords
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) => {
                StartActivity(typeof(VmmcClientSelectionActivity), typeof(VmmcRecordSelectorActivity));
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Visibility = Android.Views.ViewStates.Gone;

            var buttonViewRecordSummaries = FindViewById<Button>(Resource.Id.buttonViewRecordSummaries);
            buttonViewRecordSummaries.Click += async (sender, e) =>
            {
                await getClientSummaryReport(new VmmcLookupProvider(), Constants.VMMC_KIND_DISPLAYNAMES);
            };

            //buttonViewRecordSummaries.Click += (sender, e) =>
            //{
            //    //await getClientSummaryReport(new lspProvider(), Constants.LSP_KIND_DISPLAYNAMES);
            //    var unsyncdRecs = new store.CloudDb(Assets).GetRecordsToSync();

            //    var js = Newtonsoft.Json.JsonConvert.SerializeObject(unsyncdRecs);
            //    var bytes = System.Text.Encoding.UTF8.GetBytes(js);

            //    var b64 = string.Empty;
            //    using (var input = new System.IO.MemoryStream(bytes))
            //    {
            //        using (var outStream = new System.IO.MemoryStream())
            //        using (var deflateStream = new System.IO.Compression
            //    .DeflateStream(outStream,
            //    System.IO.Compression.CompressionMode.Compress))
            //        {
            //            input.CopyTo(deflateStream);
            //            deflateStream.Close();
            //            b64 = System.Convert.ToBase64String(outStream.ToArray());
            //        }
            //    }
            //    setTextResults(b64);
            //};
            //buttonViewRecordSummaries.Text = "Get unsynced clients";

            updateUnsyncedRecsStats();
        }
    }
}