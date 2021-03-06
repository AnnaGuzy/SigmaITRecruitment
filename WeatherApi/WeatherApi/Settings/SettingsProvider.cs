﻿namespace WeatherApi.Settings
{
    using System;

    public class SettingsProvider : ISettingsProvider
    {
        public string StorageConnectionAppSetting => Environment.GetEnvironmentVariable("StorageConnectionAppSetting");
        public string ContainerName => Environment.GetEnvironmentVariable("ContainerName");
        public string CsvDelimiter => Environment.GetEnvironmentVariable("CsvDelimiter");
    }
}