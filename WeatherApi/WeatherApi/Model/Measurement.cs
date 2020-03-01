namespace WeatherApi.Model
{
    using System.Collections.Generic;

    public class Measurement
    {
        public List<Reading> Readings { get; set; }

        public Sensor Sensor { get; set; }
    }
}