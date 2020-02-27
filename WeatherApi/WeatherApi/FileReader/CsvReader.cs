﻿using System.Collections.Generic;
using System.IO;

namespace WeatherApi.FileReader
{
    public class CsvReader<T> : ICsvReader<T>
        where T: IFromCsv, new()
    {
        readonly string delimiter;
        public CsvReader(string delimiter = ";")
        {
            this.delimiter = delimiter;
        }

        public List<T> ReadFile(Stream source)
        {
            var result = new List<T>();
            using (var reader = new StreamReader(source))
            {
                while (reader.Peek() >= 0)
                {
                    var rawValues = reader.ReadLine().Split(this.delimiter);
                    var value = new T();
                    value.MapFromArray(rawValues);
                    result.Add(value);
                }
            }

            return result;
        }
    }
}