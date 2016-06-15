using Android.App;
using Android.OS;


namespace JhpDataSystem.Modules
{
    [Activity(Label = "BrowserDataEntryActivity")]
    public class BrowserDataEntryActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application 
            var fields = Assets.Open("");
            SetContentView(Resource.Layout.DataEntryPage);
        }
    }
}