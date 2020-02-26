using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApi.Model
{
    public class Measurement
    {
        public List<Reading> Readings { get; set; }

        public Sensor Sensor { get; set; }
    }
}
