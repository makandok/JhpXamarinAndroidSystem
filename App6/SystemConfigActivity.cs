using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.store;
using JhpDataSystem.model;
using JhpDataSystem.Security;

namespace JhpDataSystem
{
    [Activity(Label = "System Cofiguration", Icon = "@drawable/jhpiego_logo")]
    public class SystemConfigActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            showAdminPage();
        }

        void showAdminPage()
        {
            SetContentView(Resource.Layout.AdminOptionsLayout);

            var buttonSaveChanges = FindViewById<Button>(Resource.Id.buttonSaveChanges);
            buttonSaveChanges.Click += SaveUserOptions_Click;

            //buttonAdminLogOut
            var buttonAdminLogOut = FindViewById<Button>(Resource.Id.buttonAdminLogOut);
            buttonAdminLogOut.Click += buttonAdminLogOut_Click;
            //(sender, e)=> { Activity.Finish(); };

            //buttonAdminViewUsers
            var buttonAdminViewUsers = FindViewById<Button>(Resource.Id.buttonAdminViewUsers);
            buttonAdminViewUsers.Click += buttonAdminViewUsers_Click;
        }

        private void doLogOut()
        {
            AppInstance.Instance.CurrentUser = null;
            var tuser = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (tuser != null)
            { tuser.Text = "Not Logged In"; }
            //we show the log in screen
            StartActivity(typeof(MainActivity));
        }

        private void buttonAdminLogOut_Click(object sender, EventArgs e)
        {
            doLogOut();
        }

        private void buttonAdminViewUsers_Click(object sender, EventArgs e)
        {
            //we get all the users
            var userCreds = (new UserAuthenticator().LoadCredentials()
                .Select(t => t.UserId + " (" + t.Names + ")")).ToList();
            userCreds.Sort();
            var asOne = string.Join(" | ", userCreds);

            //and show in a grid or alert
            showDialog("Current Users", asOne);
        }

        private void SaveUserOptions_Click(object sender, EventArgs e)
        {
            var userAuthenticator = new UserAuthenticator();
            var userCreds = userAuthenticator.LoadCredentials();

            var tNames = FindViewById<EditText>(Resource.Id.tUserNames);
            var tusername = FindViewById<EditText>(Resource.Id.tUserName);
            var tpasscode = FindViewById<EditText>(Resource.Id.tUserPassCode);
            var tpasscodAgain = FindViewById<EditText>(Resource.Id.tUserPassCodeAgain);
            if (string.IsNullOrWhiteSpace(tNames.Text) || string.IsNullOrWhiteSpace(tusername.Text)
                || string.IsNullOrWhiteSpace(tpasscode.Text) || (tpasscode.Text != tpasscodAgain.Text))
            {
                showDialog("Alert", "UserName and Passcode are both required, and Passcodes should match");
                return;
            }

            var uname = tusername.Text.ToLowerInvariant();
            var passcode = Convert.ToInt32(tpasscode.Text);
            var hash = userAuthenticator.computeHash(uname, passcode);

            var matchingCred = (from cred in userCreds
                                where cred.UserId == uname
                                select cred).FirstOrDefault();
            AppUser user = null;
            if (matchingCred == null)
            {
                //means we're ading a new user
                user = new AppUser()
                {
                    Id = new KindKey(AppInstance.Instance.LocalEntityStoreInstance.InstanceLocalDb.newId()),
                    UserId = uname,
                    Names = tNames.Text.ToUpperInvariant(),
                    KnownBolg = hash
                };
            }
            else
            {
                //confirm with the user
                user = matchingCred;
                user.KnownBolg = hash;
                user.Names = tNames.Text.ToUpperInvariant();
            }

            //we save to the database
            new DbSaveableEntity(user) { kindName = UserAuthenticator.KindName }.Save();
        }

        void showDialog(string title, string message)
        {
            new Android.App.AlertDialog.Builder(this)
            .SetTitle(title)
            .SetMessage(message)
            .SetPositiveButton("OK", (senderAlert, args) => { })
            .Create()
            .Show();
        }


    }
}