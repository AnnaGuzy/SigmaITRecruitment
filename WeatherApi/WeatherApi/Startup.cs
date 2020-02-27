using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.AzureWrapper;
using WeatherApi.FileReader;

[assembly: FunctionsStartup(typeof(WeatherApi.Startup))]

namespace WeatherApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddTransient<IBlobContainerWrapper>(x => new BlobContainerWrapper(SettingsProvider.WeatherContainer));
            builder.Services.AddSingleton(typeof(ICsvReader<>), typeof(CsvReader<>));
        }
    }
}
