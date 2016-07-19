using System;
using Android.App;
using Android.Widget;
using Android.OS;
using JhpDataSystem.projects;

namespace JhpDataSystem
{
    [Activity(Label = "Available Functionality", Icon = "@drawable/jhpiego_logo")]
    public class LauncherActivity : Activity
    {
        const string ALL_VALUES = "allValues";
        string[] DATA_CONTROLs_ARRAY = new[] { "text3",
                "text4", "text1","text2","datePicker1","jumbo1"
            };

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            showHomePage();
        }

        void showHomePage()
        {
            SetContentView(Resource.Layout.Main);
            //buttonCloseApp
            var buttonCloseApp = FindViewById<Button>(Resource.Id.buttonCloseApp);
            buttonCloseApp.Click += (x, y) =>
            {
                //throw new Exception();
                //this.FinishAffinity();
                //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                //return;

                //Device.BeginInvokeOnMainThread(() => { Android.OS.Process.KillProcess(Android.OS.Process.MyPid()); });

                this.FinishAffinity();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());                
            };

            var loggedInUserText = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (AppInstance.Instance.CurrentUser != null)
            {
                var user = AppInstance.Instance.CurrentUser;
                loggedInUserText.Text = string.Format(user.User.Names + " ({0})", user.User.UserId);
            }
            loggedInUserText.Click += showMenuLoggedInUser;

            var buttonPrepexHome = FindViewById<Button>(Resource.Id.buttonPrepexHome);
            buttonPrepexHome.Click += (x, y) =>
            {
                var contextManager = new PpxContextManager(this.Assets, this);
                if (AppInstance.Instance.ContextManager == null || AppInstance.Instance.ContextManager.ProjectCtxt == ProjectContext.Ppx)
                {
                    AppInstance.Instance.SetProjectContext(contextManager);
                    StartActivity(typeof(JhpDataSystem.projects.ppx.PPXHomeActivity));
                }
                else
                {
                    Toast.MakeText(this,
                        "Restart the app to switch projects",
                            Android.Widget.ToastLength.Long).Show();
                }
            };

            var buttonVmmcHome = FindViewById<Button>(Resource.Id.buttonVmmcHome);
            buttonVmmcHome.Click += (x, y) => 
            {
                var contextManager = new VmmcContextManager(this.Assets, this);
                if (AppInstance.Instance.ContextManager == null || AppInstance.Instance.ContextManager.ProjectCtxt == ProjectContext.Vmmc)
                {
                    AppInstance.Instance.SetProjectContext(contextManager);
                    StartActivity(typeof(JhpDataSystem.projects.vmc.VmmcHomeActivity));
                }
                else
                {
                    Toast.MakeText(this,
                        "Restart the app to switch projects",
                            Android.Widget.ToastLength.Long).Show();
                }
            };
        }

        //public void DecryptTest()
        //{
        //    var encryptionKey = "plans to death star";
        //    var textToEncrypt = "Jhpiego is an international, non-profit health organization affiliated with The Johns Hopkins University. For 40 years and in over 155 countries, Jhpiego has worked to prevent the needless deaths of women and their families";
        //    var plainText = new PlainText(textToEncrypt);
        //    var encryptedText = plainText.Encrypt(encryptionKey);
        //    var decryptedText = encryptedText.Decrypt(encryptionKey);
        //    var matches = plainText.Value == decryptedText.Value;
        //}

        void showDialog(string title, string message)
        {
            new Android.App.AlertDialog.Builder(this)
            .SetTitle(title)
            .SetMessage(message)
            .SetPositiveButton("OK", (senderAlert, args) => { })
            .Create()
            .Show();
        }

        private void showMenuLoggedInUser(object sender, EventArgs e)
        {
            new Android.App.AlertDialog.Builder(this)
                    .SetTitle("Confirm Action")
                    .SetMessage("Do you want to log out")
                    .SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        doLogOut();
                    })
                    .SetNegativeButton("Cancel", (a, b) => { return; })
                    .Create()
                    .Show();
        }

        private void doLogOut()
        {
            AppInstance.Instance.CurrentUser = null;
            var tuser = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (tuser != null)
            { tuser.Text = "Not Logged In"; }
            //we show the log in screen
            showLoginForm();
        }

        void showLoginForm()
        {
            StartActivity(typeof(MainActivity));
        }
    }
}