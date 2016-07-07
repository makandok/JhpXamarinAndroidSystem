using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using System.Linq;
using JhpDataSystem.store;
using Android.Content;
using JhpDataSystem.Utilities;

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
                //var selectedClient = showClientSelectionDialog();
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
            buttonClientsToSms.Click += (sender, e) =>
            {
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
            //we get the number of unsync'd records
            var unsyncdRecs = new CloudDb(Assets).GetRecordsToSync();
            var buttonServerSync = FindViewById<Button>(Resource.Id.buttonDatastoreSync);
            buttonServerSync.Text = string.Format("Save to Server. {0} unsaved", unsyncdRecs.Count);
            buttonServerSync.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Performing action requested", Android.Widget.ToastLength.Short).Show();
                await AppInstance.Instance.CloudDbInstance.EnsureServerSync();
                //var res = await new got(Assets).trainAriaStark();
                //var resString = res == null ? "RES IS NULL" : res.ToString();
                Toast.MakeText(this, "Sync completed", Android.Widget.ToastLength.Short).Show();
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

                //create events for them and their accompanying text fields
                dateSelectButton.Click += (a, b) =>
                {
                    var dateViewId = context.Resources.GetIdentifier(
                        Constants.DATE_TEXT_PREFIX + field.name, "id", context.PackageName);
                    var sisterView = FindViewById<EditText>(dateViewId);
                    if (sisterView == null)
                        return;
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
    }
}