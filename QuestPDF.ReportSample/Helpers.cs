using System;
using System.IO;
using SkiaSharp;

namespace QuestPDF.ReportSample
{
    public static class Helpers
    {
        public static Random Random { get; } = new Random();
        
        public static string GetTestItem(string path) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", path);

        public static byte[] GetImage(string name)
        {
            var photoPath = GetTestItem(name);
            return SKImage.FromEncodedData(photoPath).EncodedData.ToArray();
        }

        public static Location RandomLocation()
        {
            return new Location
            {
                Longitude = Helpers.Random.NextDouble() * 360f - 180f,
                Latitude = Helpers.Random.NextDouble() * 180f - 90f
            };
        }
    }
}