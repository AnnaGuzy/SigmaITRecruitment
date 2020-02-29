using System.IO;

namespace WeatherApi.FileReader
{
    public interface IZipReader
    {
        Stream UnzipFile(Stream archive, string fileName);
    }
}
