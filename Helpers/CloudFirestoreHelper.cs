using Base.Helpers;
using CloudKeyFileProvider;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloudFirestore.Helpers
{
    internal class CloudFirestoreHelper
    {
        private readonly IConfiguration configuration;

        private readonly string jsonKey = "";
        private GoogleCredential googleCredential;

        public CloudFirestoreHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
            jsonKey = KeyProvider.GetCloudKey(this.configuration["KeyFilesSetting:KeyFileName:Firestore"], KeyType.GoogleCloudFirestore);
        }

        public GoogleCredential GetGoogleCredential()
        {
            try
            {
                googleCredential = GoogleCredential.FromJson(jsonKey);
                return googleCredential;
            }
            catch (Exception ex)
            {
                #region Error Catch
                Logger.PringDebug($"-----Error{ System.Reflection.MethodBase.GetCurrentMethod().Name }-----");
                Logger.PringDebug(ex.ToString());
                return null;
                #endregion
            }
        }
    }
}
