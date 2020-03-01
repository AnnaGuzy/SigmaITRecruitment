namespace WeatherApi.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using WeatherApi.AzureWrapper;
    using WeatherApi.FileReader;
    using WeatherApi.Model;

    public class WeatherDataProvider : IWeatherDataProvider
    {
        private readonly IBlobContainerWrapper blobContainer;
        private readonly IFileNameProvider fileNameProvider;
        private readonly ICsvReader<Reading> readingCsvReader;
        private readonly ICsvReader<Sensor> sensorCsvReader;
        private readonly IZipReader zipReader;

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

        public async Task<List<Sensor>> GetSensors(string testDevice, string sensorType)
        {
            await using var metadataFile =
                await this.blobContainer.DownloadBlob(this.fileNameProvider.GetMetadataFileName());
            var metadata = this.sensorCsvReader.ReadFile(metadataFile);
            return metadata.Where(x => x.Equals(testDevice, sensorType)).ToList();
        }

        public async Task<Measurement> GetMeasurement(Sensor sensor, DateTime date)
        {
            var readings = await this.TryGetReadingsFromTemporary(sensor, date);
            if (readings == null)
            {
                readings = await this.GetReadingsFromHistorical(sensor, date);
            }

            return new Measurement { Readings = readings, Sensor = sensor };
        }

        private async Task<List<Reading>> TryGetReadingsFromTemporary(Sensor sensor, DateTime date)
        {
            var temporaryFileName = this.fileNameProvider.GetTemporaryFileName(sensor, date);
            if (!await this.blobContainer.Exists(temporaryFileName))
            {
                return null;
            }

            await using var temporaryBlobFile = await this.blobContainer.DownloadBlob(temporaryFileName);
            return this.readingCsvReader.ReadFile(temporaryBlobFile);
        }

        private async Task<List<Reading>> GetReadingsFromHistorical(Sensor sensor, DateTime date)
        {
            var historicalFileName = this.fileNameProvider.GetHistoricalArchiveName(sensor);

            if (!await this.blobContainer.Exists(historicalFileName))
            {
                return null;
            }

            await using var historicalBlobFile = await this.blobContainer.DownloadBlob(historicalFileName);
            var selectedFileName = this.fileNameProvider.GetHistoricalFileName(date);
            await using var selectedFile = await this.zipReader.UnzipFile(historicalBlobFile, selectedFileName);
            return selectedFile != null ? this.readingCsvReader.ReadFile(selectedFile) : null;
        }
    }
}