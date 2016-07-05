using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Json;
using System.Net;
using System.Threading.Tasks;

namespace JhpDataSystem.Utilities
{
    //https://developer.xamarin.com/recipes/android/networking/
    internal class SmsSender
    {
        public string message { get; set; }
        public string phoneNumber { get; set; }
        public Activity appContext { get; set; }
        public void Send()
        {
            var smsHelper = Android.Telephony.SmsManager.Default;
            //pendingIntent
            smsHelper.SendTextMessage(phoneNumber, null, message, null, null);

            //Open SMS appp to send message
            //var dest = Android.Net.Uri.Parse("smsto:" + phoneNumber);
            //var intent = new Intent(Intent.ActionSendto, dest);
            //intent.PutExtra("sms_body", message);
            //appContext.StartActivity(intent);
            //Toast.MakeText(appContext, "Your message has been sent", ToastLength.Short).Show();
        }
    }

    internal class EmailSender
    {
        public string message { get; set; }
        public string messageSubject { get; set; }
        public List<string> receipients { get; set; }
        public Activity appContext { get; set; }
        public void Send()
        {
            var email = new Intent(Android.Content.Intent.ActionSend)
            .SetType("message/rfc822")
            //.PutExtra(Intent.ExtraCc, new string[] { an_email })
            .PutExtra(Intent.ExtraEmail, receipients.ToArray())
            .PutExtra(Android.Content.Intent.ExtraSubject, messageSubject)
            .PutExtra(Android.Content.Intent.ExtraText, message);
            appContext.StartActivity(email);
            Toast.MakeText(appContext, "Your message has been sent", ToastLength.Short).Show();
        }
    }

    internal class RestServiceHelper
    {
        public string baseUrl { get; set; }

        public string urlParams { get; set; }
        async void Send(Action<string> callBack)
        {
            var jsonResult = await getResource(baseUrl + urlParams);
            if (callBack != null)
            {
                callBack(jsonResult);
            }
        }

        //https://developer.xamarin.com/recipes/android/web_services/consuming_services/call_a_rest_web_service/
        async Task<JsonValue> getResource(string resourceUrl)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(resourceUrl));
            request.ContentType = "application/json";
            request.Method = "GET";
            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            {
                return await Task.Run(() => JsonObject.Load(stream));
            }
        }
    }
}