using Azure.Storage.Blobs;

using System.IO;
using System.Threading.Tasks;

using WeatherApi.Settings;

namespace WeatherApi.AzureWrapper
{
    public class BlobContainerWrapper : IBlobContainerWrapper
    {
        private readonly BlobContainerClient blobContainerClient;

        public BlobContainerWrapper(ISettingsProvider settingsProvider)
        {
            var blobServiceClient = new BlobServiceClient(settingsProvider.StorageConnectionAppSetting);
            this.blobContainerClient = blobServiceClient.GetBlobContainerClient(settingsProvider.ContainerName);
        }

        public async Task<Stream> DownloadBlob(string blobName)
        {
            var blobClient = this.blobContainerClient.GetBlobClient(blobName);
            var file = await blobClient.DownloadAsync();
            var memoryStream = new MemoryStream();
            await file.Value.Content.CopyToAsync(memoryStream);
            return memoryStream;
        }

        public async Task<bool> Exists(string blobName)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var exists = await blobClient.ExistsAsync();
            return exists.Value;
        }
    }
}
