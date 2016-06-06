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
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;
using Google.Apis.Http;

namespace JhpDataSystem.db
{
    internal class WebDb
    {
        public Google.Apis.Storage.v1.Data.Objects ListBucketContents(Google.Apis.Storage.v1.StorageService storage, string bucket)
        {
            //https://cloud.google.com/docs/authentication#code_samples
            var request = new
                Google.Apis.Storage.v1.ObjectsResource.ListRequest(storage,
                bucket);
            var requestResult = request.Execute();
            return requestResult;
        }

        public Google.Apis.Storage.v1.StorageService CreateAuthorizedClient()
        {
            return null;
        //    //https://cloud.google.com/docs/authentication#code_samples
        //    ServiceCredential credential =
        //        GoogleCredential.GetApplicationDefaultAsync().Result;
        //    // Inject the Cloud Storage scope if required.
        //    if (credential.IsCreateScopedRequired)
        //    {
        //        credential = credential.CreateScoped(new[]
        //        {
        //    StorageService.Scope.DevstorageReadOnly
        //});
        //    }
        //    return new StorageService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "DotNet Google Cloud Platform Auth Sample",
        //    });
        }

        async void saveDataToDataStore()
        {
            //GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();

            //var client = DatastoreClient.Create();

            //var keyFactory = new KeyFactory(projectId, namespaceId, "message");
            //var entity = new Entity
            //{
            //    Key = keyFactory.CreateInsertionKey(),
            //    ["created"] = DateTime.UtcNow,
            //    ["text"] = "Text of the message"
            //};
            //var transaction = client.BeginTransaction(projectId).Transaction;
            //var commitResponse = client.Commit(projectId, Mode.TRANSACTIONAL, transaction, new[] { entity.ToInsert() });
            //var insertedKey = commitResponse.MutationResults[0].Key;
        }

        private async Task RunUsingUserCredential()
        {
            var requestUri = "path to server resource we want";

           IAuthorizationCodeFlow authFLow = null;
            var userId = "";
            TokenResponse token = null;
            var authUri = "";
            System.Net.Http.HttpMessageHandler httpMessageHandler = null;
            var messageHandler = new ConfigurableMessageHandler(httpMessageHandler);

            var cancellationToken = new CancellationToken();
            cancellationToken.Register(()=> { });

            var credential = new UserCredential(authFLow, userId, token) { };
            var accessToken = await credential.GetAccessTokenForRequestAsync(authUri, cancellationToken);

            // Create the service.
            var service = new Google.Apis.Datastore.v1beta3.DatastoreService(new BaseClientService.Initializer
            {
                HttpClientInitializer=credential,
                ApplicationName = "Discovery Sample",
                ApiKey = "[YOUR_API_KEY_HERE]",
                
            });

            var httpClient = new ConfigurableHttpClient(messageHandler);

            service.HttpClientInitializer.Initialize(httpClient);

            var res = await httpClient.GetAsync(requestUri);
        }

        void mediaDOwnloader(UserCredential credential)
        {
            //https://developers.google.com/api-client-library/dotnet/guide/media_download#sample-code
            // Create the service using the client credentials.

            //var driveService = new Google.Apis.Drive.v2.DriveService();

            var storageService = new Google.Apis.Storage.v1.StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "APP_NAME_HERE"
            });
            // Get the client request object for the bucket and desired object.
            var getRequest = storageService.Objects.Get("BUCKET_HERE", "OBJECT_HERE");
            using (var fileStream = new System.IO.FileStream(
                "FILE_PATH_HERE",
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write))
            {
                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                getRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress obj)=> { }; ;
                getRequest.Download(fileStream);
            }
        }

        void mediaUploader()
        {
            ////https://developers.google.com/drive/v3/web/manage-uploads#exp-backoff
            //var fileMetadata = new File();
            //fileMetadata.Name = "My Report";
            //fileMetadata.MimeType = "application/vnd.google-apps.spreadsheet";
            //FilesResource.CreateMediaUpload request;
            //using (var stream = new System.IO.FileStream("files/report.csv",
            //                        System.IO.FileMode.Open))
            //{
            //    var driveService = new Google.Apis.Drive.v2.DriveService();
            //    request = driveService.Files.Create(
            //        fileMetadata, stream, "text/csv");
            //    request.Fields = "id";
            //    request.Upload();
            //}
            //var file = request.ResponseBody;
            //Console.WriteLine("File ID: " + file.Id);

        }

        //private async Task RunUsingServiceAccount()
        //{
        //    var requestUri = "path to server resource we want";

        //    IAuthorizationCodeFlow authFLow = null;
        //    var userId = "";
        //    TokenResponse token = null;
        //    var authUri = "";
        //    ConfigurableMessageHandler messageHandler = new ConfigurableMessageHandler();

        //    var cancellationToken = new CancellationToken();
        //    cancellationToken.Register(() => { });

        //    var credential = new UserCredential(authFLow, userId, token) { };
        //    var accessToken = await credential.GetAccessTokenForRequestAsync(authUri, cancellationToken);

        //    // Create the service.
        //    var service = new Google.Apis.Datastore.v1beta3.DatastoreService(new BaseClientService.Initializer
        //    {
        //        ApplicationName = "Discovery Sample",
        //        ApiKey = "[YOUR_API_KEY_HERE]",

        //    });

        //    var httpClient = new ConfigurableHttpClient(messageHandler);

        //    service.HttpClientInitializer.Initialize(httpClient);

        //    var res = await httpClient.GetAsync(requestUri);
        //}

    }
}