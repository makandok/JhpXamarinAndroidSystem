using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using System.Linq;

namespace JhpDataSystem.Modules.Prepex
{
    [Activity(Label = "Prepex Manager", Icon = "@drawable/jhpiego_logo")]
    public class PrepexActivity : Activity
    {
        List<int> allDataEntryViews = new List<int>() {
                Resource.Layout.prepexreg1,
                Resource.Layout.prepexreg2,
                Resource.Layout.prepexreg3,
                Resource.Layout.prepexreg4
            };

        //TextView textView1, textView2;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            showPrepexHome();
            //var 
            //textView1 = FindViewById<TextView>(Resource.Id.textView1);
            //textView2 = FindViewById<TextView>(Resource.Id.textView2);
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

            //buttonAddNew
            var buttonAddNew = FindViewById<Button>(Resource.Id.buttonAddNew);
            buttonAddNew.Click += (sender, e) => {
                showAddNewView(true);                                
            };

            //buttonEditExisting
            var buttonEditExisting = FindViewById<Button>(Resource.Id.buttonEditExisting);
            buttonEditExisting.Click += (sender, e) => {
                showEditExistingView();
            };

            //buttonViewList
            var buttonViewList = FindViewById<Button>(Resource.Id.buttonViewList);
            buttonViewList.Click += (sender, e) => {
                showViewList();                
            };

            //buttonClientsDueFor
            var buttonClientsDueFor = FindViewById<Button>(Resource.Id.buttonClientsDueFor);
            buttonClientsDueFor.Click += (sender, e) => {
                showCliwentDueForView();                
            };
        }

        private void showAddNewView(bool showNext)
        {
            var page = getNextPage(showNext);
            if (page == currentLayout)
                return;

            currentLayout = page;
            SetContentView(page);

            addDefaultNavBehaviours();
            bindDateDialogEventsForView(page);
        }

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
            SetContentView(Resource.Layout.prepexreg1);
        }

        private void showViewList()
        {
            SetContentView(Resource.Layout.prepexreg1);
        }

        private void showEditExistingView()
        {
            SetContentView(Resource.Layout.prepexreg1);
            //
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

        void saveViewData()
        {

        }

        private void addDefaultNavBehaviours()
        {
            var buttonPrev = FindViewById<Button>(Resource.Id.buttonPrevious);
            buttonPrev.Click += (sender, e) => {
                showAddNewView(false);
            };

            if (currentLayout == Resource.Layout.PrepexDataEntryEnd)
            {
                //add bahviours for Save, Finish and Add Another One
                //buttonReview
                var buttonReview = FindViewById<Button>(Resource.Id.buttonReview);
                buttonReview.Click += (sender, e) => {
                    //present aall data in one list, perhaps as an html page
                    var data = "";
                };

                //buttonDiscard
                var buttonDiscard = FindViewById<Button>(Resource.Id.buttonDiscard);
                buttonDiscard.Click += (sender, e) => {
                    //confirm and quit
new AlertDialog.Builder(this)
.SetTitle("Confirm Action")
.SetMessage("Are you sure you want to quit? Any changes will be lost")
.SetNegativeButton("Cancel", (senderAlert, args) => {  })
.SetPositiveButton("OK", (senderAlert, args) => { showPrepexHome(); })
.Create()
.Show();

                    //just quit
                };

                //buttonFinalise
                var buttonFinalise = FindViewById<Button>(Resource.Id.buttonFinalise);
                buttonFinalise.Click += (sender, e) => {
                    //we get the data
                    var data = "";

                    //save to local db

                    //an also to out db
                };
            }
            else
            {
                var buttonNext = FindViewById<Button>(Resource.Id.buttonNext);
                buttonNext.Click += (sender, e) => {
                    showAddNewView(true);
                };
            }
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