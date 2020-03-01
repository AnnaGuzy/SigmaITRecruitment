namespace WeatherApi.FileReader
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;

    public class ZipReader : IZipReader
    {
        public async Task<Stream> UnzipFile(Stream archive, string fileName)
        {
            using var zip = new ZipArchive(archive);
            var selectedDay = zip.Entries.FirstOrDefault(x => x.Name == fileName);
            if (selectedDay == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            await selectedDay.Open().CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}