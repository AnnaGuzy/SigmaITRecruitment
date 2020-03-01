namespace WeatherApi.FileReader
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IZipReader
    {
        Task<Stream> UnzipFile(Stream archive, string fileName);
    }
}