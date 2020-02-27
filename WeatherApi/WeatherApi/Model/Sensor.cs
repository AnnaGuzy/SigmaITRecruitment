using WeatherApi.FileReader;

namespace WeatherApi.Model
{
    public class Sensor : IFromCsv
    {
        public string Name { get; set; }
        public string SensorType { get; set; }

        public void MapFromArray(string[] values)
        {
            this.Name = values[0];
            this.SensorType = values[1];
        }
    }
}
