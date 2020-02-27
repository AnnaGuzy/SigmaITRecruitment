using System;
using System.IO;
using System.Threading.Tasks;

namespace WeatherApi.AzureWrapper
{
    public interface IBlobContainerWrapper
    {
        Task<Stream> DownloadBlob(string blobName);
        Task<bool> Exists(string blobName);
    }
}
