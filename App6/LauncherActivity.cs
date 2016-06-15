using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Json;
using System.Threading.Tasks;
using JhpDataSystem.db;
using JhpDataSystem.store;
using JhpDataSystem.model;
using JhpDataSystem.modules;

namespace JhpDataSystem
{
    [Activity(Label = "Available Functionality", Icon = "@drawable/jhpiego_logo")]
    public class LauncherActivity : Activity
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
            showHomePage();
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
            //var resultsView = FindViewById<TextView>(Resource.Id.textAllValues);
            //resultsView.Text = joined;
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
    }
}