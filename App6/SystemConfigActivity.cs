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

            var currentSettings = new LocalDB3().DB.Table<DeviceConfiguration>().FirstOrDefault();
            if (currentSettings == null)
            {
                var uuid = Guid.NewGuid().ToString("N");
                var tDeviceUUID = FindViewById<TextView>(Resource.Id.tDeviceUUID);
                tDeviceUUID.Text = uuid;

                var buttonSaveDeviceConf = FindViewById<Button>(Resource.Id.buttonSaveDeviceConf);
                buttonSaveDeviceConf.Click += buttonSaveDeviceConf_Click;
            }
            else
            {
                //hide the save button
                var buttonSaveDeviceConf = FindViewById<Button>(Resource.Id.buttonSaveDeviceConf);
                buttonSaveDeviceConf.Visibility = Android.Views.ViewStates.Gone;

                var tDeviceSerial = FindViewById<EditText>(Resource.Id.tDeviceSerial);
                tDeviceSerial.Text = currentSettings.Serial;

                var tDeviceUUID = FindViewById<TextView>(Resource.Id.tDeviceUUID);
                tDeviceUUID.Text = currentSettings.UUID;
            }

            //we dont want the user to do anything else if device is not yet set up
            if (currentSettings == null)
                return;

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

        private void buttonSaveDeviceConf_Click(object sender, EventArgs e)
        {
            var tDeviceUUID = FindViewById<TextView>(Resource.Id.tDeviceUUID);
            var tDeviceSerial = FindViewById<EditText>(Resource.Id.tDeviceSerial);
            if(string.IsNullOrWhiteSpace(tDeviceSerial.Text))
            {
                showDialog("Device Serial is needed", "Please enter the device serial");
                return;
            }
            else
            {
                var devconf = new DeviceConfiguration()
                {
                    Serial = tDeviceSerial.Text,
                    UUID = tDeviceUUID.Text
                };
                new LocalDB3().DB.InsertOrReplace(devconf);
                AppInstance.Instance.Configuration = 
                    new LocalDB3().DB.Table<DeviceConfiguration>().FirstOrDefault();
                if (AppInstance.Instance.Configuration != null)
                {
                    var buttonSaveDeviceConf = FindViewById<Button>(Resource.Id.buttonSaveDeviceConf);
                    buttonSaveDeviceConf.Visibility = Android.Views.ViewStates.Gone;
                    showDialog("Success", "You can log out and start using the app");
                }
                else
                    showDialog("Error", "Could not configure the device.");
            }
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
            var asOne = string.Join(System.Environment.NewLine, userCreds);

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
                Toast.MakeText(this, "Creating new user", ToastLength.Long);
                var id = AppInstance.Instance.LocalEntityStoreInstance.InstanceLocalDb.newId();
                user = new AppUser()
                {
                    Id = new KindKey(id),
                    EntityId = new KindKey(id),
                    UserId = uname,
                    Names = tNames.Text.ToUpperInvariant(),
                    KnownBolg = hash,
                    KindMetaData =
                    (new KindMetaData()
                    {
                        chksum = 1,
                        devid = AppInstance.Instance.Configuration.Serial,
                        facidx = 0
                    }
                    ).getJson()
                };
            }
            else
            {
                //confirm with the user
                Toast.MakeText(this, "User found. Updating record", ToastLength.Long);
                user = matchingCred;
                user.KnownBolg = hash;
                user.Names = tNames.Text.ToUpperInvariant();
                if (user.EntityId == null)
                    user.EntityId = user.Id;

                if (string.IsNullOrWhiteSpace(user.KindMetaData))
                {
                    user.KindMetaData =
                    (new KindMetaData()
                    {
                        chksum = 1,
                        devid = AppInstance.Instance.Configuration.Serial,
                        facidx = 0
                    }
                    ).getJson();
                }
                else
                {
                    var metadata = new KindMetaData().fromJson(new KindItem(user.KindMetaData));
                    metadata.chksum += 1;
                    user.KindMetaData = metadata.getJson();
                }
            }

            //we save to the database
            Toast.MakeText(this, "Saving to database", ToastLength.Long);
            new DbSaveableEntity(user) { kindName = UserAuthenticator.KindName }.Save();
            Toast.MakeText(this, "Changes saved", ToastLength.Long);
            //we reset the form
            tNames.Text = "";
            tusername.Text = "";
            tpasscode.Text = "";
            tpasscodAgain.Text = "";
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