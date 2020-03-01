namespace WeatherApi.FileReader
{
    using System.IO;

    public interface IZipReader
    {
        Stream UnzipFile(Stream archive, string fileName);
    }
}