namespace WeatherApi.DataAccess
{
    using System;

    using WeatherApi.Model;

    public interface IFileNameProvider
    {
        string GetHistoricalArchiveName(Sensor sensor);
        string GetHistoricalFileName(DateTime date);
        string GetMetadataFileName();
        string GetTemporaryFileName(Sensor sensor, DateTime date);
    }
}