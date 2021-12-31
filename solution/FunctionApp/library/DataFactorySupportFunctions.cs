using AdsGoFast;
using AdsGoFast.SqlServer;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdsGoFast
{
    public static class GetSQLCreateStatementFromSchemaCore
    {


        public static JObject ParseReq(HttpRequest req,Logging logging) {

            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

            return Execute(data, logging);

        }
    
        public static JObject Execute(JObject data,
                 Logging logging)
        {
       
            string _CreateStatement;
            JArray arr;

            if (data["Data"] != null)
            {
                //Need to swap logic for parquet vs sql etc
                arr = (JArray)data["Data"]["value"];
            }
            else if (data["SchemaFileName"] != null)
            {
                string _storageAccountName = data["StorageAccountName"].ToString();
                string _storageAccountContainer = data["StorageAccountContainer"].ToString();
                string _relativePath = data["RelativePath"].ToString();
                string _schemaFileName = data["SchemaFileName"].ToString();

                _storageAccountName = _storageAccountName.Replace(".dfs.core.windows.net", "").Replace("https://", "").Replace(".blob.core.windows.net", "");

                TokenCredential StorageToken = new TokenCredential(Shared._AzureAuthenticationCredentialProvider.GetAzureRestApiToken("https://" + _storageAccountName + ".blob.core.windows.net", Shared._ApplicationOptions.UseMSI));

                arr = (JArray)JsonConvert.DeserializeObject(Shared.Azure.Storage.ReadFile(_storageAccountName, _storageAccountContainer, _relativePath, _schemaFileName, StorageToken));
            }
            else
            {
                throw new System.ArgumentException("Not Valid parameters to GetSQLCreateStatementFromSchema Function!");
            }

            bool _DropIfExist = data["DropIfExist"] == null ? false : (bool)data["DropIfExist"];

            _CreateStatement = GenerateSQLStatementTemplates.GetCreateTable(arr, data["TableSchema"].ToString(), data["TableName"].ToString(), _DropIfExist);
            _CreateStatement += Environment.NewLine + "Select 1";

            JObject Root = new JObject
            {
                ["CreateStatement"] = _CreateStatement
            };

            return Root;
        }
    }
}
