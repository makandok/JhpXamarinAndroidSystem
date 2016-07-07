//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Google.Apis.Auth.OAuth2;
//using System.Security.Cryptography.X509Certificates;
////using Google.Apis.Bigquery.v2;
//using Google.Apis.Services;
//using Google.Apis.Datastore.v1beta3;

//namespace JhpDataSystem.db
//{
//    class x
//    {
//        void doNothing()
//        {
//            var serviceAccountEmail = "539621478854-imkdv94bgujcom228h3ea33kmkoefhil@developer.gserviceaccount.com";
//            var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);
//            ServiceAccountCredential credential = new ServiceAccountCredential(
//               new ServiceAccountCredential.Initializer(serviceAccountEmail)
//               {
//                   Scopes = new[] { DatastoreService.Scope.Datastore }
//               }.FromCertificate(certificate));

//            // Create the service.
//            var service = new DatastoreService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = "Jhpiego Systems",
//            });
//        }
//    }
//}