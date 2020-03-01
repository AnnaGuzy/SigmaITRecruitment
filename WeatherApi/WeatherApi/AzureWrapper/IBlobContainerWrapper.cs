namespace WeatherApi.AzureWrapper
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IBlobContainerWrapper
    {
        Task<Stream> DownloadBlob(string blobName);
        Task<bool> Exists(string blobName);
    }
}