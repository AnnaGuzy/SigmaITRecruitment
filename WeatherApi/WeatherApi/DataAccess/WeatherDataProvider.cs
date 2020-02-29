using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.AzureWrapper;
using WeatherApi.FileReader;
using WeatherApi.Model;

namespace WeatherApi.DataAccess
{
    public class WeatherDataProvider : IWeatherDataProvider
    {
        readonly IBlobContainerWrapper blobContainer;
        readonly ICsvReader<Sensor> sensorCsvReader;
        readonly ICsvReader<Reading> readingCsvReader;
        readonly IFileNameProvider fileNameProvider;
        readonly IZipReader zipReader;
        public WeatherDataProvider(
            IBlobContainerWrapper blobContainer,
            ICsvReader<Sensor> sensorCsvReader,
            ICsvReader<Reading> readingCsvReader,
            IFileNameProvider fileNameProvider,
            IZipReader zipReader)
        {
            this.zipReader = zipReader;
            this.fileNameProvider = fileNameProvider;
            this.readingCsvReader = readingCsvReader;
            this.sensorCsvReader = sensorCsvReader;
            this.blobContainer = blobContainer;
        }

        public async Task<List<Sensor>> GetSensors(string testdevice, string sensorType)
        {
            using var metadataFile = await this.blobContainer.DownloadBlob(this.fileNameProvider.GetMetadataFileName());
            var metadata = this.sensorCsvReader.ReadFile(metadataFile);
            return metadata.Where(x => x.Equals(testdevice, sensorType)).ToList();
        }

        public async Task<Measurement> GetMeasurement(Sensor sensor, DateTime date)
        {
            var readings = await this.TryGetReadingsFromTemporary(sensor, date);
            if (readings == null)
            {
                readings = await this.GetReadingsFromHistorical(sensor, date);
            }

            return new Measurement { Readings = readings, Sensor = sensor};
        }

        private async Task<List<Reading>> TryGetReadingsFromTemporary(Sensor sensor, DateTime date)
        {
            var temporaryFileName = this.fileNameProvider.GetTemporaryFileName(sensor, date);
            if (await this.blobContainer.Exists(temporaryFileName))
            {
                using var temporaryBlobFile = await this.blobContainer.DownloadBlob(temporaryFileName);
                return this.readingCsvReader.ReadFile(temporaryBlobFile);
            }

            return null;
        }

        private async Task<List<Reading>> GetReadingsFromHistorical(Sensor sensor, DateTime date)
        {
            var historicalFileName = this.fileNameProvider.GetHistoricalArchiveName(sensor);

            if (await this.blobContainer.Exists(historicalFileName))
            {
                using var historicalBlobFile = await this.blobContainer.DownloadBlob(historicalFileName);
                var selectedFileName = this.fileNameProvider.GetHistoricalFileName(date);
                using var selectedFile = this.zipReader.UnzipFile(historicalBlobFile, selectedFileName);
                if (selectedFile != null)
                {
                    return this.readingCsvReader.ReadFile(selectedFile);
                }
            }

            return null;
        }
    }
}
