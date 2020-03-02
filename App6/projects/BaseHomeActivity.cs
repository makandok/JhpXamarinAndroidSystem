using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using JhpDataSystem.model;
using JhpDataSystem.store;
using Android.Widget;
using System.Threading.Tasks;
using JhpDataSystem.Utilities;

namespace JhpDataSystem.projects
{
    public class BaseHomeActivity<T> : Activity where T : class, ILocalDbEntity, new()
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            showDefaultHome();

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
            ////var unsyncdRecs = new CloudDb(Assets).GetRecordsToSync();
            ////var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            ////buttonServerSync.Text = string.Format("Save to Server. {0} unsaved", unsyncdRecs.Count);
            var buttonServerSync = updateUnsyncedRecsStats();
            buttonServerSync.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Performing action requested", Android.Widget.ToastLength.Short).Show();
                await AppInstance.Instance.CloudDbInstance.EnsureServerSync(
                    new WaitDialogHelper(this, sendToast));

                updateUnsyncedRecsStats();
                Toast.MakeText(this, "Completed performing action requested", Android.Widget.ToastLength.Short).Show();
            };
        }

        public Button updateUnsyncedRecsStats()
        {
            var unsyncdRecs = new CloudDb(Assets).GetRecordsToSync();
            var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            buttonServerSync.Text = string.Format("Upload ({0} available)", unsyncdRecs.Count);
            return buttonServerSync;
        }

        public void StartActivity(Type activityType, Type resultActivity)
        {
            var returnTypeString = Newtonsoft.Json.JsonConvert.SerializeObject(resultActivity);
            var intent = new Intent(this, activityType);
            intent.PutExtra(Constants.BUNDLE_NEXTACTIVITY_TYPE, returnTypeString);
            intent.SetFlags(ActivityFlags.ClearTop);
            StartActivityForResult(intent, 0);
        }

        protected virtual List<BaseWorkflowController> getActivityWFControllers()
        {
            throw new NotImplementedException();
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

                var wfControllers = getActivityWFControllers();
                var kindActivityTypes = getActivitiesForKind(new KindName(recSumm.KindName),
                            wfControllers
                    );

                if (kindActivityTypes == null)
                    return;

                var kindActivityType = kindActivityTypes.First();

                var intent = new Intent(this, kindActivityType);
                intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);
                intent.PutExtra(Constants.BUNDLE_DATATOEDIT, jsonRecord.Value);
                intent.SetFlags(ActivityFlags.ClearTop);
                StartActivityForResult(intent, 0);
            }
            else if (data.HasExtra(Constants.BUNDLE_SELECTEDCLIENT))
            {
                //result is from client selector
                var nextResultActivity = data.GetStringExtra(Constants.BUNDLE_NEXTACTIVITY_TYPE);
                var nextResultType = Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(nextResultActivity);

                var clientString = data.GetStringExtra(Constants.BUNDLE_SELECTEDCLIENT);

                var intent = new Intent(this, nextResultType);
                intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);

                StartActivityForResult(intent, 0);
            }
        }

        protected virtual void showDefaultHome()
        {
            throw new NotImplementedException();
        }

        protected void sendToast(string message, ToastLength length)
        {
            this.RunOnUiThread(()=> Toast.MakeText(this, message, length).Show());
            //Toast.MakeText(this, message, length).Show();
        }

        protected List<KindDefinition> getKindDefinition(List<BaseWorkflowController> workflowControllers)
        {
            var kindDefinitions = new List<KindDefinition>();
            foreach (var workflowController in workflowControllers)
            {
                var viewActivity = workflowController.MyActivities.FirstOrDefault().Value;
                var formsActivity =
                    (Activator.CreateInstance(viewActivity) as DataFormsBaseAttributes)
                    .InitialiseAttributes();

                var kindViews = new KindViewDefinition[workflowController.MyLayouts.Length];
                for (int indx = 0; indx < workflowController.MyLayouts.Length; indx++)
                {
                    var viewId = workflowController.MyLayouts[indx];
                    var activityType = workflowController.MyActivities[viewId];
                    kindViews[indx] = new KindViewDefinition() { ViewActivity = activityType, ViewId = viewId };
                }

                var kindDef = new KindDefinition()
                {
                    _kindName = formsActivity._kindName,
                    KindViews = kindViews
                };
                kindDefinitions.Add(kindDef);
            }
            return kindDefinitions;
        }

        protected Type[] getActivitiesForKind(KindName kindName, List<BaseWorkflowController> workflowControllers)
        {
            var kindDefinitions = getKindDefinition(workflowControllers);
            var kindDefinition = kindDefinitions.Where(t => t.Matches(kindName)).FirstOrDefault();
            if (kindDefinition == null)
                return null;

            return kindDefinition.KindViews.Select(t => t.ViewActivity).ToArray();
        }

        protected T getClientFromIntent(Intent data)
        {
            var clientString = data.GetStringExtra(Constants.BUNDLE_SELECTEDCLIENT);
            return Newtonsoft.Json.JsonConvert
                .DeserializeObject<T>(clientString);
        }

        protected void setTextResults(string asString)
        {
            var textRecordSummaries = FindViewById<TextView>(Resource.Id.textRecordSummaries);
            if (textRecordSummaries != null)
            {
                textRecordSummaries.Text = asString;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected async Task<int> getClientSummaryReport(ClientLookupProvider<T> lookupProvider
            , Dictionary<string, string> kindDisplayNames)
        {
            //or we can write to file and email
            //var file = AppInstance.Instance.writeToFile("stuff to write");

            var countRes = AppInstance.Instance.ModuleContext.GetAllBobsCount();
            var asList = (from item in countRes
                          let displayItem = new NameValuePair()
                          {
                              Name = kindDisplayNames[item.Name],
                              Value = item.Value
                          }
                          select displayItem.toDisplayText()).ToList();
            var resList = new List<string>() {
                Resources.GetString(Resource.String.sys_summary_blobcount),
                NameValuePair.getHeaderText() };
            resList.AddRange(asList);

            //also add client summary
            var recCount = lookupProvider.GetCount();
            var clientSummaryCount = new NameValuePair()
            {
                Name = "Client Summary",
                Value = recCount.ToString()
            };

            resList.Add(clientSummaryCount.toDisplayText());

            var summaryInfo = new LocalDB3().DB.Query<NameValuePair>(
                                string.Format(
                "select KindName as Name, count(*) as Value from {0} group by KindName",
                Constants.KIND_DERIVED_RECORDSUMMARY)
                );
            resList.Add(System.Environment.NewLine);
            resList.Add("Summary of Records in " + Constants.KIND_DERIVED_RECORDSUMMARY);
            var asStringList = (from nvp in summaryInfo
                                select nvp.toDisplayText()).ToList();
            resList.AddRange(asStringList);
            var asText = string.Join(System.Environment.NewLine, resList);
            setTextResults(asText);
            return 0;
        }
    }
}