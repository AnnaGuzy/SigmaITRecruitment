namespace WeatherApi.Settings
{
    public interface ISettingsProvider
    {
        string StorageConnectionAppSetting { get; }
        string ContainerName { get; }
        string CsvDelimiter { get; }
    }
}