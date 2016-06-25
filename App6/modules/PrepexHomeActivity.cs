using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using System.Linq;
using Android.Views;
using Newtonsoft.Json;
using JhpDataSystem.store;
using Android.Runtime;
using Android.Content;
using JhpDataSystem.Utilities;
using JhpDataSystem.db;

namespace JhpDataSystem.modules
{
    [Activity(Label = "Prepex Manager", Icon = "@drawable/jhpiego_logo")]
    public class PrepexHomeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            showPrepexHome();
        }

        void showPrepexHome()
        {
            currentLayout = -1;
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
            buttonUnscheduled.Click += (sender, e) => {
                StartActivity(typeof(PP_Unscheduled1));
            };
            var buttonDeviceRemovalVisit = FindViewById<Button>(Resource.Id.buttonDeviceRemovalVisit);
            buttonDeviceRemovalVisit.Click += (sender, e) => {
                StartActivity(typeof(PP_DeviceRemoval1));
            };

            var buttonPostRemovalVisit = FindViewById<Button>(Resource.Id.buttonPostRemovalVisit);
            buttonPostRemovalVisit.Click += (sender, e) => {
                var intent = new Intent(this, typeof(PP_PostRemovalVisit1));
                //StartActivity(typeof(PP_PostRemovalVisit1));
                StartActivity(intent);
            };

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                StartActivity(typeof(FilteredGridDisplayActivity));
            };

            //buttonClientsDueFor
            var buttonClientsToCall = FindViewById<Button>(Resource.Id.buttonClientsToCall);
            buttonClientsToCall.Click += (sender, e) => {
                getClientsToCall();                
            };

            var buttonClientsToSms = FindViewById<Button>(Resource.Id.buttonClientsToSms);
            buttonClientsToSms.Click += (sender, e) => {
                new SmsSender()
                { appContext = this, message = "Message from JHP", phoneNumber = "0977424090" }
                .Send();

               //getClientsToSms();
            };

            var buttonSupplies = FindViewById<Button>(Resource.Id.buttonSupplies);
            buttonSupplies.Click += (sender, e) => {
                new EmailSender() {appContext=this, message="This is an email from phone" ,
                    messageSubject="Test email 90965",receipients=new List<string>() {
                        "makandok@gmail.com", "makandok@yahoo.com"
                    } 
                }.Send();
                //getPrepexSuppliesReport();
            };

            var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            buttonServerSync.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Performing action requested", Android.Widget.ToastLength.Short).Show();

                var res = await new got(Assets).trainAriaStark();
                var resString = res == null ? "RES IS NULL" : res.ToString();
                Toast.MakeText(this, "Save Results Received. Key: "+ resString, Android.Widget.ToastLength.Long).Show();
            };
        }

        private void getPrepexSuppliesReport()
        {
            
        }

        private void getClientsToSms()
        {
            StartActivity(typeof(SmsActivity));
        }

        private void getClientsToCall()
        {
            
        }

        //private void showAddNewView(bool showNext)
        //{
        //    var page = getNextPage(showNext);
        //    if (page == currentLayout)
        //        return;

        //    currentLayout = page;
        //    SetContentView(page);

        //    addDefaultNavBehaviours();
        //    bindDateDialogEventsForView(page);
            
        //}

        private void bindDateDialogEventsForView(int viewId)
        {
            //we get all the relevant fields for this view
            var viewFields = GetFieldsForView(viewId);

            //we find the date fields
            var dateFields = (from field in viewFields
                              where field.dataType == Constants.DATEPICKER
                              select field).ToList();
            var context = this;
            //Android.Content.Res.Resources res = context.Resources;
            //string recordTable = res.GetString(Resource.String.RecordsTable);
            foreach (var field in dateFields)
            {
                //we convert these into int Ids
                int resID = context.Resources.GetIdentifier(
                    Constants.DATE_BUTTON_PREFIX + field.name, "id", context.PackageName);
                if (resID == 0)
                    continue;

                var dateSelectButton = FindViewById<Button>(resID);
                if (dateSelectButton == null)
                    continue;

                //create events for them and their accompanyinng text fields
                dateSelectButton.Click += (a, b) =>
                {
                    var dateViewId = context.Resources.GetIdentifier(
                        Constants.DATE_TEXT_PREFIX + field.name, "id", context.PackageName);
                    var sisterView = FindViewById<EditText>(dateViewId);
                    if (sisterView == null)
                        return;
                    //sisterView.Text = "text set by date click";
                    var frag = DatePickerFragment.NewInstance((time) =>
                    {
                        sisterView.Text = time.ToLongDateString();
                    });
                    frag.Show(FragmentManager, DatePickerFragment.TAG);
                };
            }
        }

        private void getDataForView(int viewId)
        {
            //we get all the relevant fields for this view
            var viewFields = GetFieldsForView(viewId);

            //we find the date fields
            var dataFields = (from field in viewFields
                              where field.dataType == Constants.DATEPICKER
                              || field.dataType == Constants.EDITTEXT
                              || field.dataType == Constants.CHECKBOX
                              || field.dataType == Constants.RADIOBUTTON
                              select field).ToList();
            var context = this;
            var valueFields = new List<FieldValuePair>();
            foreach (var field in dataFields)
            {
                var resultObject = new FieldValuePair() {Field = field, Value = string.Empty };
                switch (field.dataType)
                {
                    case Constants.DATEPICKER:
                        {
                            var view = field.GetDataView<EditText>(this);
                            if (string.IsNullOrWhiteSpace(view.Text))
                                continue;

                            resultObject.Value = view.Text;
                           break;
                        }
                    case Constants.EDITTEXT:
                        {
                            var view = field.GetDataView<EditText>(this);
                            if (string.IsNullOrWhiteSpace(view.Text))
                                continue;

                            resultObject.Value = view.Text;
                            break;
                        }
                    case Constants.CHECKBOX:
                        {
                            var view = field.GetDataView<CheckBox>(this);
                            if (!view.Checked)
                            {
                                continue;
                            }
                            resultObject.Value = Constants.DEFAULT_CHECKED;
                            break;
                        }
                    case Constants.RADIOBUTTON:
                        {
                            var view = field.GetDataView<RadioButton>(this);
                            if (!view.Checked)
                            {
                                continue;
                            }
                            resultObject.Value = Constants.DEFAULT_CHECKED;
                            break;
                        }
                    default:
                        {
                            throw new ArgumentNullException("Could not find view for field " + field.name);
                        }
                }

                if (string.IsNullOrWhiteSpace(resultObject.Value))
                {
                    throw new ArgumentNullException("Could not find view for field " + field.name);
                }
                valueFields.Add(resultObject);
            }

            AppInstance.Instance.TemporalViewData[viewId] = valueFields;
        }

        private List<FieldItem> GetFieldsForView(int viewId)
        {
            var filterString = string.Empty;
            switch (viewId)
            {
                case Resource.Layout.prepexreg1:
                    filterString = Constants.PP_VIEWS_1;
                    break;
                case Resource.Layout.prepexreg2:
                    filterString = Constants.PP_VIEWS_2;
                    break;
                case Resource.Layout.prepexreg3:
                    filterString = Constants.PP_VIEWS_3;
                    break;
                case Resource.Layout.prepexreg4:
                    filterString = Constants.PP_VIEWS_4;
                    break;
            }
            var fields = (AppInstance.Instance.FieldItems.Where(t => t.pageName == filterString)).ToList();
            return fields;
        }

        private void showCliwentDueForView()
        {
            //SetContentView(Resource.Layout.prepexreg1);
            //ClientSummaryActivity
        }

        private void showViewList()
        {
            //we show all the clients
            var currentIndexes = LocalEntityStore.Instance
                .GetAllBlobs(new KindName(Constants.KIND_PREPEX));
            if(currentIndexes.Count()==1 && currentIndexes.FirstOrDefault().Value == Constants.DBSAVE_ERROR)
            {
                //means we couldn get this data, so we throw exeption
                new ProcessLogger().Log("Could not load data from table "+Constants.KIND_PREPEX);
                new AlertDialog.Builder(this)
.SetTitle("List of clients")
.SetMessage("Error retrieving list of clients")
.SetPositiveButton("OK", (senderAlert, args) => { })
.Create()
.Show();
                return;
            }

            var allClients = currentIndexes.Select(t=> DbSaveableEntity.fromJson<PrepexDataSet>(t));
            //we display, perhaps in a listview
            var allData = (from pp in allClients
                           from field in pp.FieldValues
                           select field.Name + ": " + field.Value).ToList();
            var message = string.Join(System.Environment.NewLine, allData);
            new AlertDialog.Builder(this)
.SetTitle("List of clients")
.SetMessage(message)
.SetPositiveButton("OK", (senderAlert, args) => { })
.Create()
.Show();

        }

        private void showEditExistingView()
        {
            //SetContentView(Resource.Layout.prepexreg1);            
        }

        int currentLayout = -1;

        int getNextPage(bool getNext)
        {
            int nextLayout = -1;
            switch (currentLayout)
            {
                case -1:
                    nextLayout =
                        getNext ?
                        Resource.Layout.prepexreg1 :
                        nextLayout = Resource.Layout.prepexreg1;
                    break;
                case Resource.Layout.prepexreg1:
                    nextLayout =
                    getNext ?
                        Resource.Layout.prepexreg2 :
                        nextLayout = Resource.Layout.prepexreg1;
                    break;
                case Resource.Layout.prepexreg2:
                    nextLayout =
                    getNext ?
                        Resource.Layout.prepexreg3 :
                        nextLayout = Resource.Layout.prepexreg1;
                    break;
                case Resource.Layout.prepexreg3:
                    nextLayout =
                    getNext ?
                        Resource.Layout.prepexreg4 :
                        nextLayout = Resource.Layout.prepexreg2;
                    break;
                case Resource.Layout.prepexreg4:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntryEnd :
                        nextLayout = Resource.Layout.prepexreg3;
                    break;
                case Resource.Layout.PrepexDataEntryEnd:
                    nextLayout =
                        getNext ?
                        Resource.Layout.PrepexDataEntryEnd :
                        nextLayout = Resource.Layout.prepexreg4;
                    break;
                default:
                    {
                        break;
                    }
            }
            return nextLayout;
        }

        private void addDiscardFunctionality()
        {
            var buttonDiscard = FindViewById<Button>(Resource.Id.buttonDiscard);
            buttonDiscard.Click += (sender, e) =>
            {
                //confirm and quit
                new AlertDialog.Builder(this)
                .SetTitle("Confirm Action")
                .SetMessage("Are you sure you want to quit? Any changes will be lost")
                .SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                })
                .SetPositiveButton("OK", (senderAlert, args) =>
                {
                    showPrepexHome();
                })
                .Create()
                .Show();
            };
        }

        //private void addDefaultNavBehaviours()
        //{
        //    var buttonPrev = FindViewById<Button>(Resource.Id.buttonPrevious);
        //    buttonPrev.Click += (sender, e) =>
        //    {
        //        showAddNewView(false);
        //    };

        //    if (currentLayout == Resource.Layout.PrepexDataEntryEnd)
        //    {
        //        //add bahviours for Save, Finish and Add Another One
        //        //buttonReview
        //        var buttonReview = FindViewById<Button>(Resource.Id.buttonReview);
        //        buttonReview.Click += (sender, e) =>
        //        {
        //            //present aall data in one list, perhaps as an html page
        //            displayTemporalDataAvailable();
        //        };

        //        //buttonDiscard
        //        addDiscardFunctionality();
        //        //just quit

        //        //buttonFinalise
        //        var buttonFinalise = FindViewById<Button>(Resource.Id.buttonFinalise);
        //        buttonFinalise.Click += (sender, e) =>
        //        {
        //            //we get the data
        //            var data = getFormData();
        //            var saveable = new PrepexDataSet()
        //            {                        
        //                Id = new KindKey(LocalEntityStore.Instance.InstanceLocalDb.newId()),
        //                FormName = Constants.KIND_PREPEX_CLIENTEVAL,
        //                FieldValues = data,
        //            };

        //            //save to local db
        //            new DbSaveableEntity(saveable) { kindName = new KindName(saveable.FormName) }
        //            .Save();

        //            //and also to lookups db
        //            var lookupEntry = new PrepexDataSet()
        //            {
        //                Id = new KindKey(LocalEntityStore.Instance.InstanceLocalDb.newId()),
        //                FormName = Constants.KIND_PREPEX_CLIENTEVAL,
        //                FieldValues = getIndexedFormData(),
        //            };

        //            var ppclient = new PrepexClientSummary().Load(lookupEntry);
        //            new LocalDB3().DB.InsertOrReplace(ppclient);
        //            //new DbSaveableEntity(lookupEntry) {
        //            //    kindName = new KindName(Constants.KIND_PREPEX) }.Save();

        //            //we close and show the prepex home page
        //            showPrepexHome();
        //        };
        //    }
        //    else
        //    {
        //        var buttonNext = FindViewById<Button>(Resource.Id.buttonNext);
        //        var viewid = currentLayout;
        //        buttonNext.Click += (sender, e) =>
        //        {
        //            //we get the values
        //            getDataForView(viewid);
        //            showAddNewView(true);
        //        };
        //    }
        //}

        List<NameValuePair> getIndexedFormData()
        {
            var fields = AppInstance.Instance.TemporalViewData;
            return (from viewData in fields
                    from fieldData in viewData.Value
                    where fieldData.Field.IsIndexed
                    let rec = fieldData.AsNameValuePair()
                    select rec).ToList();
        }

        List<NameValuePair> getFormData(bool useDisplayLabels = false)
        {
            var fields = AppInstance.Instance.TemporalViewData;
            return (from viewData in fields
                    from fieldData in viewData.Value
                    let rec = useDisplayLabels?fieldData.AsLabelValuePair(): fieldData.AsNameValuePair()
                    select rec).ToList();
        }

        private void displayTemporalDataAvailable()
        {
            var fields = AppInstance.Instance.TemporalViewData;
            var nameValuePairs = getFormData(useDisplayLabels : true).Select(t => t.Name + ": " + t.Value);
            var message = string.Join(
            System.Environment.NewLine, nameValuePairs);
            new AlertDialog.Builder(this)
.SetTitle("Confirm Action")
.SetMessage(message)
.SetPositiveButton("OK", (senderAlert, args) => { })
.Create()
.Show();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            //if (BigBundle != null)
            //{
            //    outState.PutBundle(ALL_VALUES, BigBundle);
            //}
        }
    }
}