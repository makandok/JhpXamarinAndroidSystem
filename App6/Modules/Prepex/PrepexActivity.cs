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
                Resource.Layout.prepexreg1,
                Resource.Layout.prepexreg2,
                Resource.Layout.prepexreg3,
                Resource.Layout.prepexreg4
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