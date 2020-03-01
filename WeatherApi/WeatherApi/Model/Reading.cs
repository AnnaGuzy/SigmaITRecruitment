namespace WeatherApi.Model
{
    using WeatherApi.FileReader;

    public class Reading : IFromCsv
    {
        public string Time { get; set; }
        public string Value { get; set; }

        public void MapFromArray(string[] values)
        {
            this.Time = values[0];
            this.Value = values[1];
        }
    }
}