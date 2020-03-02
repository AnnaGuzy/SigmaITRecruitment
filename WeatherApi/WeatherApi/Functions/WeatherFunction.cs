namespace WeatherApi.Functions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    using WeatherApi.DataAccess;

    public class WeatherFunction
    {
        private readonly ILogger<WeatherFunction> logger;
        private readonly IWeatherDataProvider weatherDataProvider;

        public WeatherFunction(ILogger<WeatherFunction> logger,IWeatherDataProvider weatherDataProvider)
        {
            this.logger = logger;
            this.weatherDataProvider = weatherDataProvider;
        }

        [FunctionName("GetData")]
        public async Task<IActionResult> GetData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "devices/{testDevice}/{date:datetime}/{sensorType?}")]
            HttpRequest req,
            string testDevice,
            DateTime date,
            string sensorType)
        {
            this.logger.LogInformation($"Processing request of {testDevice}/{(string.IsNullOrEmpty(sensorType) ? "all types" : sensorType)} at {date.ToShortDateString()}.");
            var sensors = await this.weatherDataProvider.GetSensors(testDevice, sensorType);
            if (sensors.Any() == false)
            {
                return new BadRequestObjectResult("Given sensor does not exist.");
            }

            var measurements = await Task.WhenAll(
                sensors
                    .Select(x => this.weatherDataProvider.GetMeasurement(x, date))
                    .ToList());

            var measurementsWithReadings = measurements.Where(x => (x.Readings != null) && x.Readings.Any()).ToList();

            return measurementsWithReadings.Any()
                ? (IActionResult) new OkObjectResult(measurementsWithReadings)
                : new NotFoundObjectResult($"There were no data collected on {date.ToShortDateString()}.");
        }
    }
}