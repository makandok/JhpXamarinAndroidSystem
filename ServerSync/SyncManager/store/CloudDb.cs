using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Datastore.v1beta3;
using Android.Content.Res;
using Google.Apis.Datastore.v1beta3.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JhpDataSystem.model;
using System.Net;
using Android.Widget;
using SyncManager.store;
using SyncManager.model;

namespace JhpDataSystem.store
{
    public class CloudDb
    {
        const string DATEADDED = "dateadded";
        public static List<string> getAllKindNames()
        {
            var allClientKinds = new List<string>();
            allClientKinds.AddRange(Constants.PPX_KIND_DISPLAYNAMES.Keys);
            allClientKinds.AddRange(Constants.VMMC_KIND_DISPLAYNAMES.Keys);
            allClientKinds.Add(Constants.KIND_APPUSERS);
            return allClientKinds;
        }
        //select __key__,kindmetadata From appusers
        //select __key__, kindmetadata From `vmmc_regandproc` where dateadded
        //select __key__, dateadded From `appusers` where dateadded >= datetime('2016-08-01T00:00:01Z')
        //select * from appusers where dateadded > datetime('2016-08-16T00:00:00.1Z')
        //select __key__, dateadded From `appusers` where dateadded >=datetime('2016-08-16T00:00:00.01Z') limit 50 offset 0 order by dateadded asc 
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*
        //Key(`__Stat_Total__`, 'total_entity_usage')
        //https://console.cloud.google.com/datastore/entities/edit?key=0%2F%7C19%2Fpp_client_devicerem%7C37%2Fname:5643a2ae27144cdaa1fe0d0e43b3864b&project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*%20from%20appusers
        //https://console.cloud.google.com/datastore/entities/edit?key=0%2F%7C8%2Fappusers%7C37%2Fname:524d91cffb024c7085148004cc47854c&project=jhpzmb-vmmc-odk&ns=&kind=_GAE_MR_TaskPayload&gql=select%20*%20from%20appusers
        //https://console.cloud.google.com/datastore/entities/query/gql?project=jhpzmb-vmmc-odk&ns=&kind=_AE_Backup_Information&gql=select%20*%20from%20appusers%20order%20by%20dateadded%20asc%20limit%20500%20offset%200%20where%20dateadded%20%3E%3D%20datetime(%272016-08-16T00:00:00.01Z%27)

        private long getLastSyncedDateForKind(KindName kindName)
        {
            var db = new CloudLocalStore(kindName);
            var entity = db.GetLatestEntity();
            if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            {
                return new DateTime(2016, 07, 08, 0, 0, 0, 1).ToBinary();
            }
            return entity.EditDate;
        }

        private async Task<Key> Save(DbSaveableEntity saveableEntity)
        {
            var assets = AppInstance.Instance.ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var entity = new Entity()
            {
                Key = new Key()
                {
                    PartitionId = new PartitionId() { NamespaceId = "", ProjectId = projectId },
                    Path = new List<PathElement>() { new PathElement() {
                        Kind = saveableEntity.kindName.Value,
                        Name = saveableEntity.Id.Value}
                    }
                },
                Properties = new Dictionary<string, Value>()
                {
                    {"id", new Value() { StringValue = saveableEntity.Id.Value  } },
                    {"entityid", new Value() { StringValue = saveableEntity.EntityId.Value } },
                    {"dateadded", new Value() { TimestampValue = DateTime.Now } },
                    {"datablob", new Value() {ExcludeFromIndexes=true,
                        StringValue =saveableEntity
                    .getJson()
                    .Encrypt()
                    .Value}
                    },
                    {"kindmetadata", new Value() { StringValue = saveableEntity.Entity.KindMetaData??string.Empty } }
                }
            };
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);
            return await SaveToCloud(datastore, projectId, entity);
        }

        private async Task<Key> SaveToCloud(DatastoreService datastore, string projectId, Entity entity)
        {
            var trxbody = new BeginTransactionRequest();
            var beginTrxRequest = datastore.Projects.BeginTransaction(trxbody, projectId);
            var res = await beginTrxRequest.ExecuteAsync();
            var trxid = res.Transaction;

            var commitRequest = new CommitRequest();
            commitRequest.Mutations = new List<Mutation>() {
                new Mutation() {   Upsert = entity }
            };
            commitRequest.Transaction = trxid;
            var commmitReq = datastore.Projects.Commit(commitRequest, projectId);

            var commitExec = await commmitReq.ExecuteAsync();
            var res1 = commitExec.MutationResults.FirstOrDefault();
            return res1.Key;
        }

        private ServiceAccountCredential GetDefaultCredential(Dictionary<string, string> assets)
        {
            //System.IO.Stream mstream = null;
            //var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential.FromStream(mstream);
            var cert =
                SyncManager.Properties.Resources.key;
                //assetManager.Open(assets[Constants.ASSET_P12KEYFILE]).toByteArray();
            var serviceAccountEmail = assets[Constants.ASSET_NAME_SVC_ACCTEMAIL];
            var certificate = new X509Certificate2(cert, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(
              new ServiceAccountCredential.Initializer(serviceAccountEmail)
              {
                  Scopes = new[] { DatastoreService.Scope.Datastore }
              }.FromCertificate(certificate));
            return credential;
        }

        private DatastoreService GetDatastoreService(ServiceAccountCredential credential, Dictionary<string, string> assets)
        {
            return new DatastoreService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = assets[Constants.ASSET_NAME_APPNAME],
            });
        }

        private int AddToOutQueue(DbSaveableEntity saveableEntity)
        {
            var asString = saveableEntity.getJson();
            var outEntity = new OutEntity()
            {
                Id = saveableEntity.Id.Value,
                DataBlob = asString
            };
            var res = 0;
                //new OutDb().DB.InsertOrReplace(outEntity);
            return res;
        }

        private async Task<bool> checkConnection()
        {
            var googleUrl = "https://google.co.zm";
            var toReturn = false;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(googleUrl);
                request.Timeout = 5000;
                WebResponse response;
                response = await request.GetResponseAsync();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                toReturn = false;
            }
            catch (Exception e)
            {
                toReturn = false;
            }
            return toReturn;
        }

        int isRunning = 0;

        public Dictionary<string, string> ApiAssets { get; set; }

        public class FetchCloudDataWorker
        {
            RunQueryRequest query;
            ProjectsResource.RunQueryRequest res;

            DatastoreService _dataStore;
            string _projectId;
            KindName _kindName;
            ResultsLimit _skip;
            long _editDate;

            public FetchCloudDataWorker(DatastoreService dataStore, string projectId, KindName kindName, ResultsLimit skip, long editDate)
            {
                query = getQueryRequest(skip, editDate, kindName);
                res = dataStore.Projects.RunQuery(query
                    , projectId);
                
                _dataStore = dataStore;
                _projectId = projectId;
                _kindName = kindName;
                _skip = skip;
                _editDate = editDate;
            }

            public async Task<List<CloudEntity>> beginFetchCloudData()
            {
                var toReturn = new List<CloudEntity>();
                var fetched = false;                
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        var fetchedData = await fetchCloudData(_dataStore, _projectId, _kindName, _skip, _editDate);
                        toReturn.AddRange(fetchedData);
                        fetched = true;
                    }
                    catch (Google.GoogleApiException gex)
                    {
                        //todo: mark this record as bad to prevent it blocking for life
                        //cloudDb.InsertOrReplace(new OutEntityUnsynced().load(outEntity));
                        //cloudDb.Delete<OutEntity>(saveable.Id.Value);
                        //break;
                    }
                    catch (System.Net.WebException wex)
                    {
                        //perhaps lost connection
                        //we alllow it to spin for now
                    }
                    catch (Exception ex)
                    {
                        //ex.Message
                        //"A task was canceled."

                        //unknown exception
                    }
                    if (fetched)
                    {
                        break;
                    }
                    else
                    {
                        //lets add a 2 second delay in case it failed the first time
                        //lets log that we had to wait, try x
                        await Task.Delay(TimeSpan.FromMilliseconds(2000));
                    }
                }
                return toReturn;
            }

            private async Task<List<CloudEntity>> fetchCloudData(
                DatastoreService dataStore, string projectId,
                KindName kindName, ResultsLimit skip, long editDate)
            {
                var toReturn = new List<CloudEntity>();
                while (true)
                {
                    var response = await res.ExecuteAsync();
                    var entityResults = response.Batch.EntityResults;
                    //Debug.
                    if (entityResults == null)
                    {
                        break;
                    }

                    query.Query.StartCursor = response.Batch.EndCursor;
                    foreach (var entityResult in response.Batch.EntityResults)
                    {
                        var entity = entityResult.Entity;
                        var path = entity.Key.Path.FirstOrDefault();
                        var cloudEntity = new CloudEntity()
                        {
                            FormName = path.Kind,
                            Id = path.Name,
                            EntityId = entity.Properties["entityid"].StringValue,
                            DataBlob = entity.Properties["datablob"].StringValue,
                            KindMetaData = entity.Properties["kindmetadata"].StringValue,
                            EditDate = Convert.ToInt64(
                                entity.Properties["editdate"].IntegerValue),
                            EditDay = Convert.ToInt32(
                                entity.Properties["editday"].IntegerValue)
                        };
                        toReturn.Add(cloudEntity);
                    }

                    //moreResultsAfterLimit
                    if (response.Batch.MoreResults == "NO_MORE_RESULTS")
                    {
                        break;
                    }
                }
                return toReturn;
            }

            private RunQueryRequest getQueryRequest(ResultsLimit skip, long editDate, KindName kindName = null)
            {
                var kindExpression = kindName == null ?
                    new List<KindExpression>() :
                    new List<KindExpression> { new KindExpression() { Name = kindName.Value } };
                //var newDate = dateAdded.AddMilliseconds(10);
                var queryParams = new List<GqlQueryParameter>() {
                            new GqlQueryParameter() { Value =
                            new Value() { IntegerValue = editDate } },
                    new GqlQueryParameter() { Value =
                            new Value() { IntegerValue = skip.Value } } };
                //var t = dateAdded.toYMDInt();
                var queryString = string.Format(
                    "select * from {0} where dateadded > @1 order by dateadded ASC limit @2",
                    kindName.Value, 
                    //newDate.ToString("yyyy-MM-ddTHH:mm:ss.ffZ"), 
                    skip.Value
                    );

                var queryString1 = string.Format(
    "select * from {0} where editdate > {1} order by dateadded ASC limit {2}",
    kindName.Value,
    editDate,
    skip.Value
    );

                return new RunQueryRequest()
                {
                    //Query = new Query()
                    //{
                    //    Filter = new Google.Apis.Datastore.v1beta3.Data.Filter()
                    //    {
                    //        PropertyFilter = new PropertyFilter()
                    //        {
                    //            Property = new PropertyReference() { Name = DATEADDED },
                    //            Value = new Value() { TimestampValue = dateAdded },
                    //            //Op = "GREATER_THAN",
                    //            Op = "GREATER_THAN_OR_EQUAL",
                    //            //Op = "GREATER_THAN"
                    //        }
                    //    },
                    //    Order = new List<PropertyOrder>() {
                    //    new PropertyOrder() { Direction="ASCENDING",
                    //        Property =new PropertyReference() {Name= DATEADDED } }
                    //},
                    //    Kind = kindExpression,
                    //    Limit = skip.Value,
                    //},
                    GqlQuery = new GqlQuery()
                    {
                        QueryString = queryString1,
                        AllowLiterals = true,
                        //PositionalBindings = queryParams
                    },
                    ReadOptions = new ReadOptions() { }
                };
            }
        }

        private async Task<int> fetchRecordsForKind(KindName kindName, string projectId, DatastoreService datastore)
        {
            var skip = 50;
            var lastDateForKind = getLastSyncedDateForKind(kindName);

            var cloudEntities = await new FetchCloudDataWorker(datastore,
                projectId,
                kindName,
                new ResultsLimit(skip),
                lastDateForKind).beginFetchCloudData();
            if (cloudEntities.Count > 0)
            {
                var savedToLocal = await addToProcessingQueue(kindName, cloudEntities);
            }
            return 0;
        }

        private async Task<bool> addToProcessingQueue(KindName kindName, List<CloudEntity> cloudEntities)
        {
            var kindStore = new CloudLocalStore(kindName);
            foreach(var entity in cloudEntities)
            {
                kindStore.Update(entity);
            }
            return true;
        }

        private async Task<int> doServerSync(Action<int> updateProgress)
        {
            var currProgress = 0;
            var hasConnection = await checkConnection();
            if (!hasConnection)
            {
                isRunning = 0;
                updateProgress(100);
                return 0;
            }

            var assets = ApiAssets;
            var projectId = assets[Constants.ASSET_PROJECT_ID];
            var datastore = GetDatastoreService(GetDefaultCredential(assets), assets);

            var kindNames = getAllKindNames();

            updateProgress(currProgress += 5);
            var progressStep = 70 / kindNames.Count;
            var checkConnStep = 25 / kindNames.Count;

            foreach (var kind in kindNames)
            {
                await fetchRecordsForKind(kind.toKind(), projectId, datastore);
                            
                updateProgress(currProgress += progressStep);
                
                //check if we have a connection
                hasConnection = await checkConnection();
                updateProgress(currProgress += checkConnStep);
            }
            updateProgress(100);
            return 0;
        }

        public async Task<int> EnsureServerSync(Action<int> setProgress)
        {
            var progress = new System.Progress<int>();
            progress.ProgressChanged += (sender, e) => { setProgress(e); };
            var asIProgress = progress as IProgress<int>;

            if (isRunning == 1)
                return 0;
            isRunning = 1;
            await Task.Run(async () => await doServerSync(asIProgress.Report)
            );

            isRunning = 0;
            return 0;
        }
    }
}