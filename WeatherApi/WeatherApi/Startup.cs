using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using WeatherApi.AzureWrapper;
using WeatherApi.DataAccess;
using WeatherApi.FileReader;
using WeatherApi.Settings;

[assembly: FunctionsStartup(typeof(WeatherApi.Startup))]

namespace WeatherApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<ISettingsProvider, SettingsProvider>();
            builder.Services.AddTransient<IBlobContainerWrapper, BlobContainerWrapper>();
            builder.Services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
            builder.Services.AddSingleton<IFileNameProvider, FileNameProvider>();
            builder.Services.AddSingleton<IZipReader, ZipReader>();
            builder.Services.AddTransient<IWeatherDataProvider, WeatherDataProvider>();
        }
    }
}
