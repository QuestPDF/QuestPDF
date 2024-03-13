using System;
using System.Collections.Concurrent;
using System.IO;
using SkiaSharp;

namespace QuestPDF.ReportSample
{
    public static class Helpers
    {
        public static Random Random { get; } = new Random(1);
        
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

        private static readonly ConcurrentDictionary<int, string> RomanNumeralCache = new ConcurrentDictionary<int, string>();

        public static string FormatAsRomanNumeral(this int number)
        {
            switch (number)
            {
                case < 0:
                case > 3999:
                    throw new ArgumentOutOfRangeException(nameof(number), "Number should be in range from 1 to 3999");
                default:
                    return RomanNumeralCache.GetOrAdd(number, x =>
                    {
                        return x switch
                        {
                            >= 1000 => "M"  + (x - 1000).FormatAsRomanNumeral(),
                            >=  900 => "CM" + (x -  900).FormatAsRomanNumeral(),
                            >=  500 => "D"  + (x -  500).FormatAsRomanNumeral(),
                            >=  400 => "CD" + (x -  400).FormatAsRomanNumeral(),
                            >=  100 => "C"  + (x -  100).FormatAsRomanNumeral(),
                            >=   90 => "XC" + (x -   90).FormatAsRomanNumeral(),
                            >=   50 => "L"  + (x -   50).FormatAsRomanNumeral(),
                            >=   40 => "XL" + (x -   40).FormatAsRomanNumeral(),
                            >=   10 => "X"  + (x -   10).FormatAsRomanNumeral(),
                            >=    9 => "IX" + (x -    9).FormatAsRomanNumeral(),
                            >=    5 => "V"  + (x -    5).FormatAsRomanNumeral(),
                            >=    4 => "IV" + (x -    4).FormatAsRomanNumeral(),
                            >=    1 => "I"  + (x -    1).FormatAsRomanNumeral(),
                            _ => string.Empty,
                        };
                    });
            }
        }
    }
}