using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using JhpDataSystem.store;
using JhpDataSystem.model;
using JhpDataSystem.Security;

namespace JhpDataSystem
{
    [Activity(Label = "Jhpiego Zambia", MainLauncher = true, Icon = "@drawable/jhpiego_logo")]
    public class MainActivity : Activity
    {
        //Theme = "@android:style/Theme.DeviceDefault.Dialog.NoActionBar", 
        string ProjectId = string.Empty;
        string DataStoreApplicationKey = string.Empty;
        Bundle BigBundle = null;
        const string ALL_VALUES = "allValues";
        string[] DATA_CONTROLs_ARRAY = new[] { "text3",
                "text4", "text1","text2","datePicker1","jumbo1"
            };
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (BigBundle != null)
            {
                outState.PutBundle(ALL_VALUES, BigBundle);
            }
        }
       
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AppInstance.Instance.InitialiseAppResources(Assets);
            //we initialise the app key for our data store
            ProjectId = AppInstance.Instance.ApiAssets[Constants.ASSET_PROJECT_ID];
            DataStoreApplicationKey = AppInstance.Instance.ApiAssets[Constants.ASSET_DATASTORE_APPKEY];

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.UserLoginLayout);
            var loginFormButton = FindViewById<Button>(Resource.Id.buttonLoginIn);
            loginFormButton.Click += doLoginIn_Click;
        }

        void showDialog(string title, string message)
        {
            new AlertDialog.Builder(this)
            .SetTitle(title)
            .SetMessage(message)
            .SetPositiveButton("OK", (senderAlert, args) => { })
            .Create()
            .Show();
        }

        private void doLoginIn_Click(object sender, EventArgs e)
        {
            var fuser = new UserAuthenticator().Authenticate("mkabila", 123456);
            AppInstance.Instance.CurrentUser = fuser;
            StartActivity(typeof(LauncherActivity));
            return;

            var tusername = FindViewById<EditText>(Resource.Id.tUserName);
            var tpasscode = FindViewById<EditText>(Resource.Id.tPassCode);

            var uname = tusername.Text;
            if (string.IsNullOrWhiteSpace(uname) || string.IsNullOrWhiteSpace(tpasscode.Text))
            {
                showDialog("Alert", "UserName and Passcode are both required");
                return;
            }

            var passcode = Convert.ToInt32(tpasscode.Text);

            var authenticator = new UserAuthenticator();
            var user = authenticator.Authenticate(uname, passcode);
            if (user != null)
            {
                //we set this as the logged in user
                AppInstance.Instance.CurrentUser = user;
                if (user.User.UserId == Constants.ADMIN_USERNAME)
                {
                    //we show the admin view
                    StartActivity(typeof(SystemConfigActivity));
                }
                else
                {
                    //load the main view and update current user options
                    StartActivity(typeof(LauncherActivity));
                }
            }
            else
            {
                showDialog("Login Unsuccessful", "Could not log you in. Please check input supplied");
            }
        }
    }
}

