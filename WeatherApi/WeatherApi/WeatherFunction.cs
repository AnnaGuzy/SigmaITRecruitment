using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WeatherApi
{
    public static class WeatherFunction
    {
        [FunctionName("Weather")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var sensorType = req.Query["sensorType"];
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("StorageConnectionAppSetting"));
            var a = blobServiceClient.GetBlobContainerClient("iotbackend");
            var b = a.GetBlobClient("metadata.csv");

            var download = await b.DownloadAsync();
            var metadata = new List<Reading>();
            using (StreamReader reader = new StreamReader(download.Value.Content))
            {
                while(reader.Peek() >= 0)
                {
                    var values = reader.ReadLine().Split(";");
                    metadata.Add(new Reading { Time = values[0], Value = values[1] });
                }
                
            }

            return new OkObjectResult(metadata);
        }
    }
}
