using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.AzureWrapper;
using WeatherApi.Model;

namespace WeatherApi
{
    public class WeatherFunction
    {
        readonly IBlobContainerWrapper blobContainer;
        public WeatherFunction(IBlobContainerWrapper blobContainer)
        {
            this.blobContainer = blobContainer;
        }
        
        [FunctionName("GetData")]
        public async Task<IActionResult> GetData(
            ILogger log,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/{testdevice}/{date:datetime}/{sensorType?}")] HttpRequest req,
            string testdevice,
            DateTime date,
            string sensorType)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var metadata = await this.GetMetadata();
            var selectedSensors = metadata.Where(x => string.Equals(x.Name, testdevice, StringComparison.OrdinalIgnoreCase) && (string.Equals(x.SensorType, sensorType, StringComparison.OrdinalIgnoreCase) || sensorType == null)).ToList();
            if(selectedSensors.Any() == false)
            {
                return new BadRequestObjectResult("Given sensor does not exist.");
            }
            
            var measurements = new List<Measurement>();
            foreach (var sensor in selectedSensors)
            {
                var readings = new List<Reading>();
                var temporaryFileName = $"{sensor.Name}/{sensor.SensorType}/{date.Date.ToString("yyyy-MM-dd")}.csv";
                if (await this.blobContainer.Exists(temporaryFileName))
                {
                    var temporaryBlobFile = await this.blobContainer.DownloadBlob(temporaryFileName);
                    using (var reader = new StreamReader(temporaryBlobFile))
                    {
                        while (reader.Peek() >= 0)
                        {
                            var values = reader.ReadLine().Split(";");
                            readings.Add(new Reading { Time = values[0], Value = values[1] });
                        }
                    }
                }
                else
                {
                    var historicalFileName = $"{sensor.Name}/{sensor.SensorType}/historical.zip";

                    if (await this.blobContainer.Exists(historicalFileName))
                    {
                        var historicalBlobFile = await this.blobContainer.DownloadBlob(historicalFileName);
                        using (var zip = new ZipArchive(historicalBlobFile))
                        {
                            var selectedDay = zip.Entries.FirstOrDefault(x => x.Name == $"{date.Date.ToString("yyyy-MM-dd")}.csv");
                            if (selectedDay == null)
                            {
                                return new BadRequestObjectResult("There are no measurements in selected day");
                            }
                            else
                            {
                                using (var selectedDayStream = selectedDay.Open())
                                {
                                    using (var reader = new StreamReader(selectedDayStream))
                                    {
                                        while (reader.Peek() >= 0)
                                        {
                                            var values = reader.ReadLine().Split(";");
                                            readings.Add(new Reading { Time = values[0], Value = values[1] });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult("Given sensor does not exist.");
                    }
                }

                measurements.Add(new Measurement { Readings = readings, Sensor = sensor });
            }

            return new OkObjectResult(measurements);
        }

        private async Task<List<Sensor>> GetMetadata()
        {
            var metadataFile = await this.blobContainer.DownloadBlob(@"metadata.csv");
            var metadata = new List<Sensor>();
            using (var reader = new StreamReader(metadataFile))
            {
                while (reader.Peek() >= 0)
                {
                    var values = reader.ReadLine().Split(";");
                    metadata.Add(new Sensor { Name = values[0], SensorType= values[1] });
                }
            }

            return metadata;
        }
    }
}
