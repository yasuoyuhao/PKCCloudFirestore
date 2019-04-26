using CloudFirestore.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CloudFirestore.Services
{
    public class CloudFirestoreService
    {
        private readonly IConfiguration configuration;
        private readonly GoogleCredential googleCredential;
        private readonly Channel channel;

        public CloudFirestoreService(IConfiguration configuration)
        {
            this.configuration = configuration;
            googleCredential = new CloudFirestoreHelper(configuration).GetGoogleCredential();
            channel = new Channel(
                FirestoreClient.DefaultEndpoint.Host,
                FirestoreClient.DefaultEndpoint.Port,
                googleCredential.ToChannelCredentials()
                );
        }

        public async Task<DocumentReference> CloudHealthAsync()
        {
            FirestoreClient client = FirestoreClient.Create(channel);
            FirestoreDb db = await FirestoreDb.CreateAsync(configuration["GCPSetting:PROJECTNAME"], client);

            CollectionReference collection = db.Collection("tests");

            var testsData = new
            {
                TestsID = Guid.NewGuid().ToString(),
                CreateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            DocumentReference document = await collection.AddAsync(testsData);

            return document;
        }

        public async Task<Google.Cloud.Firestore.WriteResult> AddFieldsbyDictionaryAsync(IDictionary<string, object> datas, string collectionName, string documentName)
        {
            FirestoreClient client = FirestoreClient.Create(channel);
            FirestoreDb db = await FirestoreDb.CreateAsync(configuration["GCPSetting:PROJECTNAME"], client);

            Google.Cloud.Firestore.WriteResult result;

            var document = db.Collection(collectionName).Document(documentName);
            var snapshot = await document.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                result = await document.UpdateAsync(datas);
            }
            else
            {
                result = await document.CreateAsync(datas);
            }

            return result;
        }

        public async Task<Google.Cloud.Firestore.WriteResult> AddFieldbyObjectAsync(object data, string collectionName, string documentName)
        {
            FirestoreClient client = FirestoreClient.Create(channel);
            FirestoreDb db = await FirestoreDb.CreateAsync(configuration["GCPSetting:PROJECTNAME"], client);

            Google.Cloud.Firestore.WriteResult result;

            var document = db.Collection(collectionName).Document(documentName);

            var snapshot = await document.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                result = await document.SetAsync(data);
            }
            else
            {
                result = await document.CreateAsync(data);
            }

            return result;
        }

        public async Task<DocumentSnapshot> FetchData(string collectionName, string documentName)
        {
            FirestoreClient client = FirestoreClient.Create(channel);
            FirestoreDb db = await FirestoreDb.CreateAsync(configuration["GCPSetting:PROJECTNAME"], client);

            var document = db.Collection(collectionName).Document(documentName);

            var snapshot = await document.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot;
            }

            return null;
        }

        public async Task<Google.Cloud.Firestore.WriteResult> DeleteFieldData(string collectionName, string documentName, List<string> keys)
        {
            FirestoreClient client = FirestoreClient.Create(channel);
            FirestoreDb db = await FirestoreDb.CreateAsync(configuration["GCPSetting:PROJECTNAME"], client);

            var document = db.Collection(collectionName).Document(documentName);

            var snapshot = await document.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> updates = new Dictionary<string, object>();
                keys.ForEach(key =>
                {
                    updates.Add(key, FieldValue.Delete);
                });

                Google.Cloud.Firestore.WriteResult writeResult = await document.UpdateAsync(updates);

                return writeResult;
            }

            return null;
        }

        ~CloudFirestoreService()
        {
            channel.ShutdownAsync().Wait();
        }
    }
}
