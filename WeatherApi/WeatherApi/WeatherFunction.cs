using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using WeatherApi.AzureWrapper;
using WeatherApi.FileReader;
using WeatherApi.Model;

namespace WeatherApi
{
    public class WeatherFunction
    {
        readonly ILogger<WeatherFunction> logger;
        readonly IBlobContainerWrapper blobContainer;
        readonly ICsvReader<Sensor> sensorCsvReader;
        readonly ICsvReader<Reading> readingCsvReader;
        public WeatherFunction(
            ILogger<WeatherFunction> logger,
            IBlobContainerWrapper blobContainer,
            ICsvReader<Sensor> sensorCsvReader,
            ICsvReader<Reading> readingCsvReader)
        {
            this.readingCsvReader = readingCsvReader;
            this.sensorCsvReader = sensorCsvReader;
            this.logger = logger;
            this.blobContainer = blobContainer;
        }
        
        [FunctionName("GetData")]
        public async Task<IActionResult> GetData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/{testdevice}/{date:datetime}/{sensorType?}")] HttpRequest req,
            string testdevice,
            DateTime date,
            string sensorType)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");
            var metadata = await this.GetMetadata();
            var selectedSensors = metadata.Where(x => string.Equals(x.Name, testdevice, StringComparison.OrdinalIgnoreCase) && (string.Equals(x.SensorType, sensorType, StringComparison.OrdinalIgnoreCase) || sensorType == null)).ToList();
            if(selectedSensors.Any() == false)
            {
                return new BadRequestObjectResult("Given sensor does not exist.");
            }
            
            var measurements = new List<Measurement>();
            foreach (var sensor in selectedSensors)
            {
                List<Reading> readings;
                var temporaryFileName = $"{sensor.Name}/{sensor.SensorType}/{date.Date.ToString("yyyy-MM-dd")}.csv";
                if (await this.blobContainer.Exists(temporaryFileName))
                {
                    var temporaryBlobFile = await this.blobContainer.DownloadBlob(temporaryFileName);
                    readings = this.readingCsvReader.ReadFile(temporaryBlobFile);
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
                                    readings = this.readingCsvReader.ReadFile(selectedDayStream);
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
            return this.sensorCsvReader.ReadFile(metadataFile);
        }
    }
}
