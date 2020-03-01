using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.Model;

namespace WeatherApi.DataAccess
{
    public interface IWeatherDataProvider
    {
        Task<List<Sensor>> GetSensors(string testDevice, string sensorType);
        Task<Measurement> GetMeasurement(Sensor sensor, DateTime date);
    }
}
