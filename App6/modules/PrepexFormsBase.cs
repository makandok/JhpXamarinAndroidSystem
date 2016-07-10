using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using System.Linq;
using JhpDataSystem.store;
using Android.Content;

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
        protected bool IsFirstPage = false;
        protected PPClientSummary CurrentClient { get; set; }
        protected KindName _kindName { get; set; }

        void showPrepexHome()
        {
            var intent = new Intent(this, typeof(PrepexHomeActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivityForResult(intent, 0);
        }

        private PPClientSummary loadClientFromIntent()
        {
            //load default data or client if is first
            var intent = this.Intent;
            if (intent.Extras != null && intent.Extras.ContainsKey(Constants.BUNDLE_SELECTEDCLIENT))
            {
                var clientString = intent.Extras.GetString(Constants.BUNDLE_SELECTEDCLIENT);
                var ppclientSummary = Newtonsoft.Json.
                    JsonConvert.DeserializeObject<PPClientSummary>(clientString);
                CurrentClient = ppclientSummary;
                return ppclientSummary;
            }
            return null;
        }

        protected void ShowMyView()
        {                       
            SetContentView(myView);
            addDefaultNavBehaviours();
            bindDateDialogEventsForView(myView);
            loadClientFromIntent();

            if (IsFirstPage && CurrentClient != null)
            {
                //if requires selection of client, we show the client selection dialog 
                //loadClientFromIntent();

                //load client information if it has any indexed fields
                var viewFields = GetFieldsForView(myView);
                var indexedData = CurrentClient.ToValuesList();

                List<FieldValuePair> fvp = new List<FieldValuePair>();
                foreach (var value in indexedData)
                {
                    var field = viewFields
                        .Where(t => t.name == value.Name && t.name != Constants.FIELD_DATEOFVISIT)
                        .FirstOrDefault();
                    if (field == null)
                        continue;

                    fvp.Add(new FieldValuePair() { Field = field, Value = value.Value });
                }
                setViewData(fvp);
            }
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

        protected void setViewData(List<FieldValuePair> clientInfo)
        {
            var context = this;
            foreach (var fvp in clientInfo)
            {
                if (string.IsNullOrWhiteSpace(fvp.Value))
                    continue;

                var field = fvp.Field;
                switch (field.dataType)
                {
                    case Constants.DATEPICKER:
                    case Constants.EDITTEXT:
                        {
                            var view = field.GetDataView<EditText>(this);
                            view.Text = fvp.Value;
                            break;
                        }
                    case Constants.CHECKBOX:
                    case Constants.RADIOBUTTON:
                        {
                            var view = field.GetDataView<CheckBox>(this);
                            view.Checked = fvp.Value == "1";
                            break;
                        }
                    default:
                        {
                            throw new ArgumentNullException("Could not find view for field " + field.name);
                        }
                }
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
                    data.Add(new NameValuePair() { Name = Constants.FIELD_ID, Value = kindKey.Value });
                    var saveable = new PPDataSet()
                    {
                        Id = kindKey,
                        FormName = _kindName.Value,
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
                        var deviceSizeControl = data.Where(t => t.Name.Contains(Constants.FIELD_PPX_DEVSIZE_PREFIX)).FirstOrDefault();
                        if (deviceSizeControl != null)
                        {
                            var deviceSize = deviceSizeControl.Name.Last().ToString().ToUpperInvariant();
                            data.Add(new NameValuePair() { Name = Constants.FIELD_PPX_DEVSIZE, Value = deviceSize });
                        }
                        //we get the client lookup details and save these
                        saveClientSummary(data, saveable.EntityId);
                    }
                    else
                    {
                        //also update client details but only if they have changes
                        saveable.EntityId = CurrentClient.EntityId;
                        data.Add(new NameValuePair()
                        {
                            Name = Constants.FIELD_ENTITYID,
                            Value = CurrentClient.EntityId.Value
                        });
                    }

                    //we compute the indexed data and save
                    //saveClientSummary(data, saveable.EntityId);

                    //save to local db
                    saveable.FieldValues = data;
                    var saveableEntity = new DbSaveableEntity(saveable) { kindName = _kindName };
                    saveableEntity.Save();

                    //fire and forget
                    AppInstance.Instance.CloudDbInstance.AddToOutQueue(saveableEntity);
                    AppInstance.Instance.TemporalViewData.Clear();

                    //we show the splash screen and await results of the operation
                    //todo: show dialog when beginning server sync and await excution
                    AppInstance.Instance.CloudDbInstance.EnsureServerSync();

                    //we close and show the prpex home page
                    this.Finish();
                    showPrepexHome();
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

                    var clientString = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentClient);
                    var intent = new Intent(this, nextActivity);
                    //myView == Resource.Layout.PrepexDataEntryEnd
                    intent.PutExtra(Constants.BUNDLE_SELECTEDCLIENT, clientString);
                    StartActivityForResult(intent, 0);
                };
            }
        }

        private void saveClientSummary(List<NameValuePair> data, KindKey clientId)
        {
            var clientSummary = new PPDataSet()
            {
                Id = clientId,
                FormName = _kindName.Value,
                EntityId = clientId,
                FieldValues = getIndexedFormData(data)
            };
            var ppclient = new PPClientSummary().Load(clientSummary);
            new LocalDB3().DB.InsertOrReplace(ppclient);
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