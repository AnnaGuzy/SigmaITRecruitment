using System;
using WeatherApi.Model;

namespace WeatherApi.FileReader
{
    public interface IFileNameProvider
    {
        string GetHistoricalArchiveName(Sensor sensor);
        string GetHistoricalFileName(DateTime date);
        string GetMetadataFileName();
        string GetTemporaryFileName(Sensor sensor, DateTime date);
    }
}
