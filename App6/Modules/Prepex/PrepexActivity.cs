using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;

namespace JhpDataSystem.Modules.Prepex
{
    [Activity(Label = "Prepex Manager", Icon = "@drawable/jhpiego_logo")]
    public class PrepexActivity : Activity
    {
        List<int> allDataEntryViews = new List<int>() {
                Resource.Layout.PrepexDataEntry1,
                Resource.Layout.PrepexDataEntry2,
                Resource.Layout.PrepexDataEntry3,
                Resource.Layout.PrepexDataEntry4,
                Resource.Layout.PrepexDataEntry5
            };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
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
            if (showNext)
            {
                addDefaultNavBehaviours();
            }
        }

        private void showCliwentDueForView()
        {
            SetContentView(Resource.Layout.PrepexDataEntry1);
        }

        private void showViewList()
        {
            SetContentView(Resource.Layout.PrepexDataEntry1);
        }

        private void showEditExistingView()
        {
            SetContentView(Resource.Layout.PrepexDataEntry1);
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
                        Resource.Layout.PrepexDataEntry1 :
                        nextLayout = Resource.Layout.PrepexDataEntry1;
                    break;
                case Resource.Layout.PrepexDataEntry1:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntry2 :
                        nextLayout = Resource.Layout.PrepexDataEntry1;
                    break;
                case Resource.Layout.PrepexDataEntry2:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntry3 :
                        nextLayout = Resource.Layout.PrepexDataEntry1;
                    break;
                case Resource.Layout.PrepexDataEntry3:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntry4 :
                        nextLayout = Resource.Layout.PrepexDataEntry2;
                    break;
                case Resource.Layout.PrepexDataEntry4:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntry5 :
                        nextLayout = Resource.Layout.PrepexDataEntry3;
                    break;
                case Resource.Layout.PrepexDataEntry5:
                    nextLayout =
                    getNext ?
                        Resource.Layout.PrepexDataEntryEnd :
                        nextLayout = Resource.Layout.PrepexDataEntry4;
                    //nextLayout = Resource.Layout.PrepexDataEntryEnd;
                    break;
                case Resource.Layout.PrepexDataEntryEnd:
                    nextLayout =
                        getNext ?
                        Resource.Layout.PrepexDataEntryEnd :
                        nextLayout = Resource.Layout.PrepexDataEntry5;
                    break;
                default:
                    {
                        break;
                    }
            }
            return nextLayout;
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

                //buttonSave
                var buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
                buttonSave.Click += (sender, e) => {
                    //we get all the data
                    var data = "";
                    //and save to local db

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