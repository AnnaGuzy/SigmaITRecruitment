namespace WeatherApi.DataAccess
{
    using System;

    using WeatherApi.Model;

    public class FileNameProvider : IFileNameProvider
    {
        private const string DateFormat = "yyyy-MM-dd";

        public string GetTemporaryFileName(Sensor sensor, DateTime date)
        {
            return $"{sensor.Name}/{sensor.SensorType}/{date.ToString(DateFormat)}.csv";
        }

        public string GetHistoricalArchiveName(Sensor sensor)
        {
            return $"{sensor.Name}/{sensor.SensorType}/historical.zip";
        }

        public string GetHistoricalFileName(DateTime date)
        {
            return $"{date.ToString(DateFormat)}.csv";
        }

        public string GetMetadataFileName()
        {
            return @"metadata.csv";
        }
    }
}