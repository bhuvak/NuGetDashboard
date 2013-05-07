using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace NuGetDashboard.Utilities
{
    /// <summary>
    /// This class provides the service methods to load/process report blobs from Storage.
    /// </summary>
    public class BlobStorageService
    {
        private static string _connectionString = ConfigurationManager.AppSettings["StorageConnection"];
       
        public static string Load(string name)
        {         
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("dashboard");
            CloudBlockBlob blob = container.GetBlockBlobReference(name);
            string content;          
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                content = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());                
            }

            return content;
        }

        /// <summary>
        /// Gets the JSON data from the blob. The blobs are pre-created as key value pairs using Ops tasks.
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public static void GetJsonDataFromBlob(string blobName, out List<string> xValues, out List<Object> yValues)
        {
            xValues = new List<string>();
            yValues = new List<Object>();       
            string json = Load(blobName);
            if (json == null)
            {
                return;
            }

            JArray array = JArray.Parse(json);
            foreach (JObject item in array)
            {
                xValues.Add(item["key"].ToString());
                yValues.Add((item["value"]).ToString());
            }

        }
    }
}