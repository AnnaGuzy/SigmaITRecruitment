using System.Collections.Generic;
using System.IO;

namespace WeatherApi.FileReader
{
    public interface ICsvReader<T> where T : IFromCsv, new()
    {
        List<T> ReadFile(Stream source);
    }
}