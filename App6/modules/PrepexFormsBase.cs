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

namespace JhpDataSystem.modules
{
    public interface IPP_NavController
    {
        System.Type GetNextActivity(int currentLayout, bool moveForward);
        int GetNextLayout(int currentLayout, bool moveForward);
        System.Type GetActivityForLayout(int targetLayout);
    }

    public class PrepexFormsBase : Activity
    {
        protected bool _isRegistration = false;
        protected int myView = -1;
        protected IPP_NavController myNavController = null;
        protected bool RequiresClient = true;
        protected PrepexClientSummary CurrentClient { get; set; }

        private PrepexClientSummary showClientSelectionDialog()
        {
            return null;
        }

        protected void ShowMyView()
        {
            //if requires selection of client, we show the client selection dialog
            if (RequiresClient && CurrentClient == null)
            {
                //we show the client selection dialog
                var selectedClient = showClientSelectionDialog();
            }

            SetContentView(myView);
            addDefaultNavBehaviours();
            bindDateDialogEventsForView(myView);
        }

        protected void bindDateDialogEventsForView(int viewId)
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

        protected void getDataForView(int viewId)
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
                var resultObject = new FieldValuePair() { Field = field, Value = string.Empty };
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

        protected List<FieldItem> GetFieldsForView(int viewId)
        {
            return AppInstance.Instance.FieldItems.Where(t => t.PageId == viewId).ToList();
        }

//        protected void showViewList()
//        {
//            //we show all the clients
//            var currentIndexes = LocalEntityStore.Instance
//                .GetAllBlobs(new KindName(Constants.KIND_PREPEX));
//            if (currentIndexes.Count() == 1 && currentIndexes.FirstOrDefault().Value == Constants.DBSAVE_ERROR)
//            {
//                //means we couldn't get this data, so we throw exeption
//                new ProcessLogger().Log("Could not load data from table " + Constants.KIND_PREPEX);
//                new Android.App.AlertDialog.Builder(this)
//.SetTitle("List of clients")
//.SetMessage("Error retrieving list of clients")
//.SetPositiveButton("OK", (senderAlert, args) => { })
//.Create()
//.Show();
//                return;
//            }

//            var allClients = currentIndexes.Select(t => DbSaveableEntity.fromJson<PrepexDataSet>(t));
//            //we display, perhaps in a listview
//            var allData = (from pp in allClients
//                           from field in pp.FieldValues
//                           select field.Name + ": " + field.Value).ToList();
//            var message = string.Join(System.Environment.NewLine, allData);
//            new AlertDialog.Builder(this)
//.SetTitle("List of clients")
//.SetMessage(message)
//.SetPositiveButton("OK", (senderAlert, args) => { })
//.Create()
//.Show();

//        }

        protected void addDiscardFunctionality()
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
                    this.Finish();
                   //showPrepexHome();
                })
                .Create()
                .Show();
            };
        }

        public override void OnBackPressed()
        {
                new AlertDialog.Builder(this)
.SetTitle("Warning: Confirm action")
.SetMessage("You'll lose all data entered if you navigate backwards. Are you sure you want to navigate backwards")
.SetPositiveButton("OK", (senderAlert, args) => { base.OnBackPressed(); })
.SetNegativeButton("Cancel", (senderAlert, args) => { })
.Create()
.Show();
        }

        protected void addDefaultNavBehaviours()
        {
            var buttonPrev = FindViewById<Button>(Resource.Id.buttonPrevious);
            buttonPrev.Click += (sender, e) =>
            {
                var next = myNavController.GetNextLayout(myView, false);
                if (myView == next)
                    return;
                OnBackPressed();
            };

            if (myView == Resource.Layout.PrepexDataEntryEnd)
            {
                //add bahviours for Save, Finish and Add Another One
                //buttonReview
                var buttonReview = FindViewById<Button>(Resource.Id.buttonReview);
                buttonReview.Click += (sender, e) =>
                {
                    //present aall data in one list, perhaps as an html page
                    displayTemporalDataAvailable();
                };

                //buttonDiscard
                addDiscardFunctionality();
                //just quit

                //buttonFinalise
                var buttonFinalise = FindViewById<Button>(Resource.Id.buttonFinalise);
                buttonFinalise.Click += (sender, e) =>
                {
                    //we get the data
                    var data = getFormData();
                    if (data.Count == 0)
                    {
                        return;
                    }

                    var kindKey = new KindKey(LocalEntityStore.Instance.InstanceLocalDb.newId());
                    var kindname = new KindName(Constants.KIND_PREPEX_CLIENTEVAL);
                    var saveable = new PrepexDataSet()
                    {
                        Id = kindKey,
                        FormName = kindname.Value,
                        //FieldValues = data,
                    };

                    //save to lookups db
                    if (this.GetType() == typeof(PP_ClientEvalEnd))
                    {
                        var entityId = new KindKey(kindKey.Value);
                        saveable.EntityId = entityId;
                        data.Add(new NameValuePair() {
                            Name = Constants.FIELD_ENTITYID,
                            Value = entityId.Value });
                        //we get the device size
                        var deviceSizeControl = data.Where(t => t.Name.Contains(Constants.FIELD_PREPEX_DEVSIZE_PREFIX)).FirstOrDefault();
                        if (deviceSizeControl != null)
                        {
                            var deviceSize = deviceSizeControl.Name.Last().ToString().ToUpperInvariant();
                            data.Add(new NameValuePair() { Name = Constants.FIELD_PREPEX_DEVSIZE, Value = deviceSize });
                        }                       
                    
                        //we get the client lookup details and save these
                    }
                    else
                    {
                        //also update client details but only if they have changes

                        //consider passing along the client id
                        //var clientid = getCurrentClientId();
                        //data.Add(new NameValuePair() { Name = Constants.FIELD_ENTITYID, Value = clientid });
                    }

                    data.Add(new NameValuePair() { Name = Constants.FIELD_ID, Value = kindKey.Value });

                    saveable.FieldValues = getIndexedFormData(data);
                    var ppclient = new PrepexClientSummary().Load(saveable);
                    new LocalDB3().DB.InsertOrReplace(ppclient);

                    //save to local db
                    saveable.FieldValues = data;
                    var saveableEntity = new DbSaveableEntity(saveable) { kindName = kindname };
                    saveableEntity.Save();

                    //fire and forget
                    AppInstance.Instance.CloudDbInstance.AddToOutQueue(saveableEntity);
                    AppInstance.Instance.TemporalViewData.Clear();

                    //we show the splash screen and await results of the operation
                    //todo: show dialog when beginning server sync and await excution
                    AppInstance.Instance.CloudDbInstance.EnsureServerSync();
                    
                    //we close and show the prpex home page
                    this.Finish();
                    //showPrepexHome();
                };
            }
            else
            {
                var buttonNext = FindViewById<Button>(Resource.Id.buttonNext);
                //var viewid = currentLayout;
                buttonNext.Click += (sender, e) =>
                {
                    //we get the values
                    getDataForView(myView);
                    //showAddNewView(true);
                    var next = myNavController.GetNextLayout(myView, true);
                    if (myView == next)
                        return;
                    var nextActivity = myNavController.GetActivityForLayout(next);
                    StartActivity(nextActivity);
                };
            }
        }

        protected List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
        {
            var indexFieldNames = Constants.PP_IndexedFieldNames;
            return (data.Where(
                t => indexFieldNames.Contains(t.Name))).ToList();
        }

        //protected List<NameValuePair> getIndexedFormData()
        //{
        //    var indexedFieldName = new List<string>() {
        //        Constants.FIELD_ENTITYID,"dateofvisit", "cardserialnumber","clientidnumber","clientname","dob","clienttel",
        //        "clientsphysicaladdress"};
        //    var fields = AppInstance.Instance.TemporalViewData;
        //    return (from viewData in fields
        //            from fieldData in viewData.Value
        //            where fieldData.Field.IsIndexed
        //            let rec = fieldData.AsNameValuePair()
        //            select rec).ToList();
        //}

        protected List<NameValuePair> getFormData(bool useDisplayLabels = false)
        {
            var fields = AppInstance.Instance.TemporalViewData;
            return (from viewData in fields
                    from fieldData in viewData.Value
                    let rec = useDisplayLabels ? fieldData.AsLabelValuePair() : fieldData.AsNameValuePair()
                    select rec).ToList();
        }

        protected void displayTemporalDataAvailable()
        {
            var fields = AppInstance.Instance.TemporalViewData;
            var nameValuePairs = getFormData(useDisplayLabels: true).Select(t => t.Name + ": " + t.Value);
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