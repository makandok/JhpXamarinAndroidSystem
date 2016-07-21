using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem.store;
using Newtonsoft.Json;

namespace JhpDataSystem.projects
{
    public class DataFormsBaseAttributes: Activity
    {
        protected bool _isRegistration = false;
        protected int myView = -1;
        protected IPP_NavController myNavController = null;
        protected bool IsFirstPage = false;        
        public KindName _kindName { get; set; }
        //public bool AllowShowView = true;
        public DataFormsBaseAttributes InitialiseAttributes()
        {
            doPreCreate(null);
            return this;
        }

        protected virtual void doPreCreate(Bundle savedInstanceState)
        {
            _kindName = null; // new KindName(Constants.KIND_PPX_UNSCHEDULEDVISIT);
            myView = -1; // Resource.Layout.prepexunscheduled2;
            myNavController = null; // new PP_UnscheduledVisitControl() { };
        }
    }

    public class DataFormsBase<T>: DataFormsBaseAttributes where T : class, ILocalDbEntity, new()
    {
        protected T CurrentClient { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {           
            //call base
            base.OnCreate(savedInstanceState);

            //calling thiss here to set activity attributes
            doPreCreate(savedInstanceState);
            ShowMyView();
        }


        protected virtual bool IsRegistrationEndPage()
        {
            return false;
        }

        protected virtual Type getHomeActivityType()
        {
            return null;
        }

        protected void showHome()
        {
            var intent = new Intent(this, getHomeActivityType());
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivityForResult(intent, 0);
        }

        protected T loadClientFromIntent()
        {
            //load default data or client if is first
            var intent = this.Intent;
            if (intent.Extras != null && intent.Extras.ContainsKey(Constants.BUNDLE_SELECTEDCLIENT))
            {
                var clientString = intent.Extras.GetString(Constants.BUNDLE_SELECTEDCLIENT);
                var ppclientSummary = Newtonsoft.Json.
                    JsonConvert.DeserializeObject<T>(clientString);
                CurrentClient = ppclientSummary;
                return ppclientSummary;
            }
            return null;
        }

        //protected void addToIntent(Intent targetIntent, string bundleName, string bundleValue)
        //{
        //    targetIntent.PutExtra(bundleName, bundleValue);
        //}

        //protected string loadFromIntent(string bundleName)
        //{
        //    //load default data or client if is first
        //    var intent = this.Intent;
        //    if (intent.Extras != null && intent.Extras.ContainsKey(bundleName))
        //    {
        //        return intent.Extras.GetString(bundleName);
        //    }
        //    return string.Empty;
        //}

        protected void ShowMyView()
        {
            SetContentView(myView);
            addDefaultNavBehaviours();
            bindDateDialogEventsForView(myView);
            loadClientFromIntent();

            if(this.Intent.Extras!=null && this.Intent.Extras.ContainsKey(Constants.BUNDLE_DATATOEDIT))
            {
                //means we have dat to edit
                var jsonRecord = this.Intent.GetStringExtra(Constants.BUNDLE_DATATOEDIT);
                var saveableEntity = JsonConvert
                    .DeserializeObject<GeneralEntityDataset>(jsonRecord);
                if (saveableEntity != null)
                {
                    var viewFields = GetFieldsForView(myView);
                    var indexedData = saveableEntity.FieldValues;
                    var fvp = getNameValuePairs(viewFields, indexedData);
                    setViewData(fvp);
                }
                else
                {
                    //we abort and show the home page
                }
            }
            else if (IsFirstPage && CurrentClient != null)
            {
                //if requires selection of client, we show the client selection dialog 
                //loadClientFromIntent();

                //load client information if it has any indexed fields
                var viewFields = GetFieldsForView(myView);
                var indexedData = CurrentClient.ToValuesList();
                var fvp = getNameValuePairs(viewFields, indexedData);
                setViewData(fvp);
            }
        }

        List<FieldValuePair> getNameValuePairs(List<FieldItem> viewFields, List<NameValuePair> fieldValues)
        {
            List<FieldValuePair> fvp = new List<FieldValuePair>();
            foreach (var value in fieldValues)
            {
                //t.name != Constants.FIELD_VMMC_DATEOFVISIT || 
                var field = viewFields
                    .Where(t => t.name == value.Name && (t.name != Constants.FIELD_PPX_DATEOFVISIT))
                    .FirstOrDefault();
                if (field == null)
                    continue;
                fvp.Add(new FieldValuePair() { Field = field, Value = value.Value });
            }
            return fvp;
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
                    case Constants.RADIOBUTTON:
                        {
                            var view = field.GetDataView<RadioButton>(this);
                            view.Checked = fvp.Value == "1";
                            break;
                        }
                    case Constants.CHECKBOX:
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

        protected virtual List<FieldItem> GetFieldsForView(int viewId)
        {
            return null;
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


        protected virtual List<NameValuePair> getModuleClientSummaries(IEnumerable<NameValuePair> data)
        {
            return new List<NameValuePair>();
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

            if (myView == Resource.Layout.DataEntryEnd)
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
                buttonFinalise.Click += async (sender, e) =>
                {
                    //we get the data
                    var data = getFormData();
                    if (data.Count == 0)
                    {
                        return;
                    }

                    var kindKey = new KindKey(AppInstance.Instance.LocalEntityStoreInstance.InstanceLocalDb.newId());
                    data.Add(new NameValuePair() { Name = Constants.FIELD_ID, Value = kindKey.Value });
                    var saveable = new GeneralEntityDataset()
                    {
                        Id = kindKey,
                        FormName = _kindName.Value,
                        //FieldValues = data,
                    };

                    //save to lookups db
                    if (IsRegistrationEndPage())
                    {
                        var entityId = new KindKey(kindKey.Value);
                        saveable.EntityId = entityId;
                        data.Add(new NameValuePair()
                        {
                            Name = Constants.FIELD_ENTITYID,
                            Value = entityId.Value
                        });

                        //we get the device size
                        var moduleClientSummaries = getModuleClientSummaries(data);
                        data.AddRange(moduleClientSummaries);

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

                    var dateEdited = DateTime.Now;
                    var creationDate = data.Where(t => t.Name.Contains(Constants.SYS_FIELD_DATECREATED)).FirstOrDefault();
                    if (creationDate == null)
                    {
                        data.Add(new NameValuePair()
                        {
                            Name = Constants.SYS_FIELD_DATECREATED,
                            Value = dateEdited.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        });
                    }

                    var editDate = data.Where(t => t.Name.Contains(Constants.SYS_FIELD_DATEEDITED)).FirstOrDefault();
                    if (editDate == null)
                    {
                        data.Add(new NameValuePair()
                        {
                            Name = Constants.SYS_FIELD_DATEEDITED,
                            Value = dateEdited.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        });
                    }
                    else
                    {
                        //create an audit entry.
                        //skip for now
                    }

                    //save to local db
                    saveable.FieldValues = data;
                    var saveableEntity = new DbSaveableEntity(saveable) { kindName = _kindName };
                    saveableEntity.Save();

                    //save to record summmary
                    new LocalDB3().DB.InsertOrReplace(new RecordSummary()
                    {
                        Id = saveable.Id.Value,
                        EntityId = saveable.EntityId.Value,
                        VisitDate = dateEdited,
                        KindName = _kindName.Value
                    });

                    //fire and forget
                    AppInstance.Instance.CloudDbInstance.AddToOutQueue(saveableEntity);
                    AppInstance.Instance.TemporalViewData.Clear();

                    //we show the splash screen and await results of the operation
                    //todo: show dialog when beginning server sync and await excution
                    var syncRes = await AppInstance.Instance.CloudDbInstance.EnsureServerSync(sendToast);

                    //we close and show the prpex home page
                    this.Finish();
                    showHome();
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

        void sendToast(string message, ToastLength length)
        {
            Toast.MakeText(this, message, length).Show();
        }

        protected virtual void saveClientSummary(List<NameValuePair> data, KindKey clientId)
        {
            var clientSummary = new GeneralEntityDataset()
            {
                Id = clientId,
                FormName = _kindName.Value,
                EntityId = clientId,
                FieldValues = getIndexedFormData(data)
            };

            var ppclient = new T().Load(clientSummary) as T;
            new LocalDB3().DB.InsertOrReplace(ppclient);
        }

        protected virtual List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
        {
            return null;
        }

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
        }
    }

}