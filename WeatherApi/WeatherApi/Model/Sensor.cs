namespace WeatherApi.Model
{
    using System;

    using WeatherApi.FileReader;

    public class Sensor : IFromCsv
    {
        public string Name { get; set; }
        public string SensorType { get; set; }

        public void MapFromArray(string[] values)
        {
            this.Name = values[0];
            this.SensorType = values[1];
        }

        public bool Equals(string name, string sensorType)
        {
            return string.Equals(this.Name, name, StringComparison.OrdinalIgnoreCase)
                   && (string.IsNullOrEmpty(sensorType) ||
                       string.Equals(this.SensorType, sensorType, StringComparison.OrdinalIgnoreCase));
        }
    }
}