using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1beta3;
using Android.Content.Res;
using Google.Apis.Datastore.v1beta3.Data;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JhpDataSystem.db
{
    /// <summary>
    /// This sample demonstrates the simplest use case for a Service Account service.
    /// The certificate needs to be downloaded from the Google Developers Console
    /// <see cref="https://console.developers.google.com/">
    ///   "Create another client ID..." -> "Service Account" -> Download the certificate,
    ///   rename it as "key.p12" and add it to the project. Don't forget to change the Build action
    ///   to "Content" and the Copy to Output Directory to "Copy if newer".
    /// </summary>
    public class got
    {
       AssetManager _assetManager { get; set; }        
       public string AplicationKey { get; internal set; }
        public got(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public DatastoreService CreateDataStoreClient(Dictionary<string, string> assets)
        {
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;

            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(new[] { DatastoreService.Scope.Datastore });
            }

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME], // assets.Open(Constants.ASSET_NAME_APPNAME).toText() ,
                HttpClientInitializer = credentials
            };

            return new DatastoreService(serviceInitializer);
        }

        Key getKey(string kind)
        {
            return new Key() { Path = new[] { new PathElement() { Kind = kind } } };
        }

        List<Key> allocateIds(DatastoreService datasstore, List<Key> keys, string projectId)
        {
            var alloc = new AllocateIdsRequest() {Keys = keys };
            var worker = datasstore.Projects.AllocateIds(alloc, projectId);
            var res = worker.Execute();
            return keys;
        }


        public void unwrapAriaStark(Dictionary<string, string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];

            // Create the service.
            var datastore = GetDatastoreService(GetDefaultCredential(assets, _assetManager), assets);

            //Retrieve entities from the server
            var res = datastore.Projects.RunQuery(
                new RunQueryRequest()
                {
                    //Query = new Query(){Limit = 10,Kind = new List<KindExpression> { new KindExpression() {Name = "jhpsystems" }, }},
                    GqlQuery = new GqlQuery() { QueryString = "select __key__, cardserial From jhpsystems" }
                    ,
                    ReadOptions = new ReadOptions() { }
                }, projectId);

            var response = res.Execute();

            var y = (
            from entityResult in response.Batch.EntityResults
            let entity = entityResult.Entity
            select new { entity.Key.Path, entity.Properties.Values }).ToList();

            var batch = response.Batch.EntityResults;
        }


        public void trainAriaStark(Dictionary<string, string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];            
            // Create the service.
            var datastore = GetDatastoreService(GetDefaultCredential(assets, _assetManager), assets);

            //create the keys
            //assign ids
            var keys = allocateIds(datastore,
                new List<Key> { getKey("jhpsystems"), getKey("jhpsystems"), getKey("jhpsystems") }, projectId
                );
            //select __key__, cardserial, registerno From jhpsystems
            //assign to entities
            var entities = new List<Mutation>();
            var x = 1;
            foreach (var key in keys)
            {
                x++;
                var entity = new Entity() { Key = key };
                entity.Properties = new Dictionary<string, Value>() {
                    {"cardserial", new Value() { IntegerValue = 20*x }},
                    {"datablob", new Value() { StringValue = "This is a blob "+ x }},
                    {"registerno", new Value() { IntegerValue = 330 + x }},
                    {"dateadded", new Value() { TimestampValue = DateTime.Now }}
                };
                entities.Add(new Mutation() { Upsert = entity });
            }

            //Google.Apis.Datastore.v1beta3.Data.

            //we figure out how to save
            var firstEntity = entities[0];
            var tr = new BeginTransactionRequest() {};
            var trid= datastore.Projects.BeginTransaction(tr, projectId);
            var transactionResponse = trid.Execute();


            var mut = new Mutation() {Upsert=new Entity() };
            var cr = new CommitRequest() { Mutations = entities };

            datastore.Projects.Commit(cr, projectId);

            //perhaps we save to the local database

            //alert that we are done saving
            var t = 0;
        }

        //Key addTask(DatastoreService datastore, String description)
        //{
        //    var t = new Google.Apis.Datastore.v1beta3.Data.AllocateIdsRequest();
        //    //new Google.Apis.Datastore.v1beta3.DatastoreBaseServiceRequest()
        //    //t.Keys.Add("MKabila");

        //   //Key key = datastore.allocateId(keyFactory.newKey());
        //   // Entity task = new Entity();
        //   // var props = task.Properties;
        //   // props["description"] = new Value() { StringValue= description };
        //   // props["created"] = new Value() { TimestampValue = DateTime.Now };
        //   // props["done"] = new Value() { BooleanValue = false };
        //   // props[""] = new Value() { StringValue = description };


        //    //datastore.HttpClient.PutAsync();// .put(task);
        //    return key;
        //}



        internal ServiceAccountCredential GetDefaultCredential(Dictionary<string,string> assets, AssetManager assetManager)
        {
            var cert = assetManager.Open(assets[Constants.ASSET_P12KEYFILE]).toByteArray();
            var serviceAccountEmail = assets[Constants.ASSET_NAME_SVC_ACCTEMAIL];
            var certificate = new X509Certificate2(cert, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer(serviceAccountEmail)
              {
                  Scopes = new[] { DatastoreService.Scope.Datastore }
              }.FromCertificate(certificate));
            return credential;
        }

        internal DatastoreService GetDatastoreService(ServiceAccountCredential credential, Dictionary<string,string> assets)
        {
            return new DatastoreService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME],
            });
        }

        public void manyFacedGod(Dictionary<string,string> assets)
        {
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var credential = GetDefaultCredential(assets, _assetManager);

            // Create the service.
            var datastore = GetDatastoreService(credential, assets);


            //create the keys
            //assign ids
            var keys = allocateIds(datastore,
                new List<Key> { getKey("jhpsystems"), getKey("jhpsystems"), getKey("jhpsystems") },
                projectId
                );
            //select __key__, cardserial, registerno From jhpsystems
            //assign to entities
            var entities = new List<Mutation>();
            var x = 1;
            foreach (var key in keys)
            {
                x++;
                var entity = new Entity() { Key = key };
                entity.Properties = new Dictionary<string, Value>() {
                    {"cardserial", new Value() { IntegerValue = 20*x }},
                    {"datablob", new Value() { StringValue = "This is a blob "+ x }},
                    {"registerno", new Value() { IntegerValue = 330 + x }},
                    {"dateadded", new Value() { TimestampValue = DateTime.Now }}
                };
                entities.Add(new Mutation() { Upsert = entity });
            }

            //we figure out how to save
            var tr = new BeginTransactionRequest() { };
            var trid = datastore.Projects.BeginTransaction(tr, projectId);

            var mut = new Mutation() { Upsert = new Entity() };
            var cr = new CommitRequest() { Mutations = entities };

            datastore.Projects.Commit(cr, projectId);

            //perhaps we save to the local database

            //alert that we are done saving
            var t = 0;
        }
    }
}