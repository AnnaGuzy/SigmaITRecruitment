using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WeatherApi.AzureWrapper
{
    public class BlobContainerWrapper : IBlobContainerWrapper
    {
        private readonly BlobContainerClient blobContainerClient;

        public BlobContainerWrapper(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(SettingsProvider.StorageConnectionAppSetting);
            this.blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<Stream> DownloadBlob(string blobName)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var file = await blobClient.DownloadAsync();
            return file.Value.Content;
        }

        public async Task<bool> Exists(string blobName)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var exists = await blobClient.ExistsAsync();
            return exists.Value;
        }
    }
}
