using System.Collections.Generic;
using Android.App;
using Android.Widget;
using JhpDataSystem.model;

namespace JhpDataSystem.projects.session
{
    [Activity(Label = "Site Session Manager", Icon = "@drawable/jhpiego_logo")]
    public class SessionHomeActivity : BaseHomeActivity<SiteSession>
    {
        protected override void showDefaultHome()
        {
            showPPXHome();
        }

        protected override List<BaseWorkflowController> getActivityWFControllers()
        {
            return new List<BaseWorkflowController>();
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
                StartActivity(typeof(SessionEditor));
            };
            buttonClientEvaluation.Text = "Manage Sessions";


            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Visibility = Android.Views.ViewStates.Gone;

            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonUnscheduled.Visibility = Android.Views.ViewStates.Gone;
            //buttonDeviceRemovalVisit.Click += (sender, e) =>
            //{
            //    StartActivity(typeof(VmmcClientSelectionActivity), typeof(VmmcPostOp1));
            //};
            //buttonDeviceRemovalVisit.Text = "Post Operation";

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Visibility = Android.Views.ViewStates.Gone;
            //buttonPostRemovalVisit.Click += (sender, e) => {
            //    StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_PostRemovalVisit1));
            //};

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Visibility = Android.Views.ViewStates.Gone;
            //buttonViewList.Click += (sender, e) => {
            //    StartActivity(typeof(VmmcFilteredGridDisplayActivity));
            //};

            //buttonEditRecords
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) =>
            {
                //StartActivity(typeof(VmmcClientSelectionActivity), typeof(VmmcRecordSelectorActivity));
                sendToast("No action defined", ToastLength.Long);
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Visibility = Android.Views.ViewStates.Gone;

            var buttonViewRecordSummaries = FindViewById<Button>(Resource.Id.buttonViewRecordSummaries);
            buttonViewRecordSummaries.Click += async (sender, e) =>
            {
                //await getClientSummaryReport(new VmmcLookupProvider(), Constants.VMMC_KIND_DISPLAYNAMES);
                sendToast("No action defined", ToastLength.Long);
            };
        }
    }
}