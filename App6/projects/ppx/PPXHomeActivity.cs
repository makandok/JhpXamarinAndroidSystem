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
using JhpDataSystem.projects.ppx.activity;
using JhpDataSystem.projects.ppx.wfcontrollers;

namespace JhpDataSystem.projects.ppx
{
    [Activity(Label = "@string/ppx_activitylabel", Icon = "@drawable/jhpiego_logo")]
    public class PPXHomeActivity : BaseHomeActivity<PPClientSummary>
    {
        protected override void showDefaultHome()
        {
            showPPXHome();
        }

        protected override List<BaseWorkflowController> getActivityWFControllers()
        {
            return new List<BaseWorkflowController>()
                                {
                                    new PP_PostRemovalControl(),
                                    new PP_DeviceRemovalControl(),
                                    new PP_UnscheduledVisitControl(),
                                    new PP_ClientEvalControl()
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
                //showAddNewView(true);
                StartActivity(typeof(PP_ClientEval1));
            };

            var buttonUnscheduled = FindViewById<Button>(Resource.Id.buttonUnscheduled);
            buttonUnscheduled.Click += (sender, e) =>
            {
                StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_Unscheduled1));
            };
            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Click += (sender, e) =>
            {
                StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_DeviceRemoval1));
            };

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Click += (sender, e) => {
                StartActivity(typeof(PpxClientSelectionActivity), typeof(PP_PostRemovalVisit1));
            };

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                StartActivity(typeof(PpxFilteredGridDisplayActivity));
            };

            //buttonViewList
            var buttonEditRecords = FindViewById<Button>(Resource.Id.buttonEditRecords);
            buttonEditRecords.Click += (sender, e) => {
                StartActivity(typeof(PpxClientSelectionActivity), typeof(PpxRecordSelectorActivity));
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Click += (sender, e) => 
            {
                getPrepexSuppliesReport();
            };

            var buttonViewRecordSummaries = FindViewById<Button>(Resource.Id.buttonViewRecordSummaries);
            buttonViewRecordSummaries.Click += async (sender, e) =>
            {
                await getClientSummaryReport(new PpxLookupProvider(), Constants.PPX_KIND_DISPLAYNAMES);
            };
        }

        List<PPDeviceSizes> getPPDeviceSizes()
        {
            var allClients = new PpxLookupProvider().Get();
            var allSizes = new Dictionary<int, PPDeviceSizes>();
            foreach (var client in allClients)
            {
                if (client.PlacementDate.Year != 1900 && 
                    client.PlacementDate.Year < 2017)
                    continue;
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

        private void getPrepexSuppliesReport()
        {
            //we get all clients
            var allSizes = getPPDeviceSizes();            
            var resList = new List<string>();
            resList.Add(Resources.GetString(Resource.String.ppx_sys_deviceusage));
            resList.Add(System.Environment.NewLine);
            resList.Add(PPDeviceSizes.getHeader());
            var ordered = allSizes.OrderByDescending(t=>t.PlacementDate);

            var monthTotals = new Dictionary<int, PPDeviceSizes>();
            foreach (var dayUsage in ordered)
            {
                var month = dayUsage.PlacementDate.Month;
                var year = dayUsage.PlacementDate.Year;
                var yyyyMM = year * 100 + month;
                PPDeviceSizes sizeForMonth = null;
                if (!monthTotals.TryGetValue(yyyyMM, out sizeForMonth))
                {
                    sizeForMonth = new PPDeviceSizes(01, month, year) {isMonth = true };
                    monthTotals[yyyyMM] = sizeForMonth;
                }
                sizeForMonth.Add(dayUsage.size);
            }
            //foreach (var dayUsage in ordered)
            //{
            //    resList.Add(dayUsage.toDisplay());
            //}
            foreach (var monthUsages in monthTotals)
            {
                resList.Add(monthUsages.Value.toDisplay());
            }

            var asString = string.Join(System.Environment.NewLine, resList);
            setTextResults(asString);
        }
    }
}