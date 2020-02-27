using System;

namespace WeatherApi
{
    public static class SettingsProvider
    {
        public static string StorageConnectionAppSetting => Environment.GetEnvironmentVariable("StorageConnectionAppSetting");
        public static string WeatherContainer => Environment.GetEnvironmentVariable("WeatherContainer");
    }
}
