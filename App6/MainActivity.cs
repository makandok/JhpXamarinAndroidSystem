using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JhpDataSystem.db;
using JhpDataSystem.store;
using JhpDataSystem.model;
using JhpDataSystem.Security;
using Newtonsoft.Json;
using JhpDataSystem.Modules.Prepex;

namespace JhpDataSystem
{
    [Activity(Label = "Jhpiego Systems", MainLauncher = true, Icon = "@drawable/jhpiego_logo")]
    public class MainActivity : Activity
    {
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
            showLoginForm();
        }

        void LoadMainView(Bundle bundle)
        {            
            showHomePage();
            if (BigBundle == null)
            {
                BigBundle = new Bundle();
            }

            if (bundle != null && bundle.ContainsKey(ALL_VALUES))
            {
                BigBundle.PutAll(bundle.GetBundle(ALL_VALUES));
                var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
                if (resultsView != null)
                {
                    resultsView.Text = BigBundle.ToString();
                }
            }
        }

        void showHomePage()
        {
            SetContentView(Resource.Layout.Main);
            var loggedInUserText = FindViewById<Button>(Resource.Id.tLoggedInUser);
            if (AppInstance.Instance.CurrentUser != null)
            {
                var user = AppInstance.Instance.CurrentUser;
                loggedInUserText.Text = string.Format(user.User.Names + " ({0})", user.User.UserId);
            }
            loggedInUserText.Click += showMenuLoggedInUser;

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.SaveData);
            button.Click += buttonClicked;

            var button2 = FindViewById<Button>(Resource.Id.fetchData);
            button2.Click += getWebResource;

            var showLoginButton = FindViewById<Button>(Resource.Id.buttonPrepexHome);
            //PrepexActivity
            showLoginButton.Click += (x, y) => { StartActivity(typeof(PrepexActivity)); };


            var buttonVmmcHome = FindViewById<Button>(Resource.Id.buttonVmmcHome);
            buttonVmmcHome.Click += (x, y) => {
                var uri = Android.Net.Uri.Parse("http://www.xamarin.com");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };
            //buttonVmmcHome
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

        private void showMenuLoggedInUser(object sender, EventArgs e)
        {
            new AlertDialog.Builder(this)
                    .SetTitle("Confirm Action")
                    .SetMessage("Do you want to log out")
                    .SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        doLogOut();
                    })
                    .SetNegativeButton("Cancel", (a, b)=> { return; })
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

        private void doLoginIn_Click(object sender, EventArgs e)
        {
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
                    showAdminPage();
                }
                else
                {
                    //load the main view and update current user options
                    showHomePage();
                }
            }
            else
            {
                showDialog("Login Unsuccessful", "Could not log you in. Please check input supplied");
            }
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
                    Id = new KindKey(LocalEntityStore.Instance.InstanceLocalDb.newId()),
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

        void showLoginForm()
        {
            SetContentView(Resource.Layout.UserLoginLayout);
            var loginFormButton = FindViewById<Button>(Resource.Id.buttonLoginIn);
            loginFormButton.Click += doLoginIn_Click;
        }

        private void doGetData(object sender, EventArgs e)
        {
            //getWebResource();
        }

        private void buttonClicked(object sender, EventArgs e)
        {
            //new got(Assets) { AplicationKey = "" }.unwrapAriaStark(ApiAssets);

            new got(Assets) { AplicationKey = "" }.trainAriaStark(AppInstance.Instance.ApiAssets);

            return;
            var bundle = new Bundle();

            var asButton = sender as Button;
            if (asButton == null)
                return;

            var parent = asButton.Parent as ViewGroup;

            var jsonObject = new JsonObject();
            foreach (var controlName in DATA_CONTROLs_ARRAY)
            {
                var controlId = Resources.GetIdentifier(controlName, "id", PackageName);
                if (controlId == 0)
                    continue;

                var view = FindViewById(controlId);
                var viewType = view.GetType();

                if (viewType == typeof(EditText))
                {
                    //inputType
                    var asSpecificType = view as EditText;
                    bundle.PutString(controlName, asSpecificType.Text);
                }
                else if (viewType == typeof(DatePicker))
                {
                    var asSpecificType = view as DatePicker;
                    bundle.PutLong(controlName, asSpecificType.DateTime.ToBinary());
                }
                //else if (viewType == typeof(DatePicker))
                //{

                //}
                else
                {
                    ///we skip for now
                }
            }

            //we save to the local database
            var id = Guid.NewGuid().ToString("N");
            var myKey = LocalEntityStore.Instance.Save(new KindKey(id), new KindName(Constants.KIND_DEFAULT), new KindItem("I've been Saved"));

            //we clear the ui
            resetUi();

            updateFilterList();

            //BigBundle.Clear();
            //BigBundle.PutAll(bundle);

            //var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            //resultsView.Text = bundle.ToString();
        }

        private void updateFilterList()
        {
            //we get the keys
            var records = LocalEntityStore.Instance.GetKeys(new KindName("default"));

            //and show them in the next grid
            var joined = string.Join("\n", records);
            var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            resultsView.Text = joined;
        }

        private void resetUi()
        {
            foreach (var controlName in DATA_CONTROLs_ARRAY)
            {
                var controlId = Resources.GetIdentifier(controlName, "id", PackageName);
                if (controlId == 0)
                    continue;

                var view = FindViewById(controlId);
                var viewType = view.GetType();

                if (viewType == typeof(EditText))
                {
                    var asSpecificType = view as EditText;
                    asSpecificType.Text = "";
                }
                else if (viewType == typeof(DatePicker))
                {
                    var asSpecificType = view as DatePicker;
                    asSpecificType.DateTime = DateTime.MinValue;
                }
            }
        }

        public async void getWebResource(object sender, EventArgs e)
        {            
            ////http://www.w3schools.com/json/tryit.asp?filename=tryjson_http&url=myTutorials.txt
            //string url = "http://www.w3schools.com/json/tryit.asp?filename=tryjson_http&url=myTutorials.txt";
            //JsonValue json = await FetchDataAsync(url);
            //// ParseAndDisplay (json);

            //var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            //resultsView.Text = json.ToString();
        }

        private async Task<JsonValue> FetchDataAsync(string url)
        {
            // Create an HTTP web request using the URL:
            var url1 = new Uri(url);
            var request = System.Net.WebRequest.Create(url1);
            request.ContentType = "application/json";
            request.Method = "GET";

            //byte[] byteArray = Encoding.UTF8.GetBytes(contents);
            ////byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            //MemoryStream stream = new MemoryStream(byteArray);

            string result = string.Empty;
            try
            {
                using (var response = await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                {
                    var jsonDoc = await Task.Run(() => new System.IO.StreamReader(stream).ReadToEndAsync());
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    result = jsonDoc;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                result = Resource.String.DEFAULT_ERRORCODE.ToString();
            }
            catch
            {
                result = Resource.String.DEFAULT_ERRORCODE.ToString();
            }
            finally
            {

            }
            return result;
        }

        //[Java.Interop.Export("myClickHandler")]
        //public void doLogin(View view)
        //{
        //    //object sender, EventArgs e
        //}
    }
}

