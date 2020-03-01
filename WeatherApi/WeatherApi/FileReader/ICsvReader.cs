namespace WeatherApi.FileReader
{
    using System.Collections.Generic;
    using System.IO;

    public interface ICsvReader<T> where T : IFromCsv, new()
    {
        List<T> ReadFile(Stream source);
    }
}