﻿using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WeatherApi.FileReader
{
    public class ZipReader : IZipReader
    {
        public Stream UnzipFile(Stream archive, string fileName)
        {
            using var zip = new ZipArchive(archive);
            var selectedDay = zip.Entries.FirstOrDefault(x => x.Name == fileName);
            if (selectedDay == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            selectedDay.Open().CopyToAsync(memoryStream);
            return memoryStream;
        }
    }
}
