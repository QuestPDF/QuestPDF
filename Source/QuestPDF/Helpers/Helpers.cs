using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using static QuestPDF.Skia.SkSvgImageSize.Unit;

namespace QuestPDF.Helpers
{
    internal static class Helpers
    {
        static Helpers()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
        internal static byte[] LoadEmbeddedResource(string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new BinaryReader(stream);
            
            return reader.ReadBytes((int) stream.Length);
        }
        
        private static PropertyInfo? ToPropertyInfo<T, TValue>(this Expression<Func<T, TValue>> selector)
        {
            return (selector.Body as MemberExpression)?.Member as PropertyInfo;
        }
        
        internal static string? GetPropertyName<T, TValue>(this Expression<Func<T, TValue>> selector) where TValue : class
        {
            return selector.ToPropertyInfo()?.Name;
        }
        
        internal static TValue? GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> selector) where TValue : class
        {
            return selector.ToPropertyInfo()?.GetValue(target) as TValue;
        }
        
        internal static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> selector, TValue value)
        {
            var property = selector.ToPropertyInfo() ?? throw new Exception("Expected property with getter and setter.");
            property?.SetValue(target, value);
        }

        internal static string PrettifyName(this string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled);
        }

        internal static void VisitChildren(this Element? element, Action<Element?> handler)
        {
            if (element == null)
                return;
            
            foreach (var child in element.GetChildren())
                VisitChildren(child, handler);

            handler(element);
        }

        internal static bool IsNegative(this Size size)
        {
            return size.Width < -Size.Epsilon || size.Height < -Size.Epsilon;
        }

        internal static bool IsEmpty(this Element element)
        {
            return element.Measure(Size.Zero).Type == SpacePlanType.Empty;
        }
        
        internal static int ToQualityValue(this ImageCompressionQuality quality)
        {
            return quality switch
            {
                ImageCompressionQuality.Best => 100,
                ImageCompressionQuality.VeryHigh => 90,
                ImageCompressionQuality.High => 75,
                ImageCompressionQuality.Medium => 50,
                ImageCompressionQuality.Low => 25,
                ImageCompressionQuality.VeryLow => 10,
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }
        
        internal static bool ToDownsamplingStrategy(this ImageCompressionQuality quality)
        {
            return quality switch
            {
                ImageCompressionQuality.Best => false,
                ImageCompressionQuality.VeryHigh => false,
                ImageCompressionQuality.High => true,
                ImageCompressionQuality.Medium => true,
                ImageCompressionQuality.Low => true,
                ImageCompressionQuality.VeryLow => true,
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }
        
        internal static SkImage CompressImage(this SkImage image, ImageCompressionQuality compressionQuality)
        {
            return image.ResizeAndCompress(image.Width, image.Height, compressionQuality.ToQualityValue(), compressionQuality.ToDownsamplingStrategy());
        }

        internal static SkImage ResizeAndCompressImage(this SkImage image, ImageSize targetResolution, ImageCompressionQuality compressionQuality)
        {
            return image.ResizeAndCompress(targetResolution.Width, targetResolution.Height, compressionQuality.ToQualityValue(), compressionQuality.ToDownsamplingStrategy());
        }

        internal static SkImage GetImageWithSmallerSize(SkImage one, SkImage second)
        {
            return one.EncodedDataSize < second.EncodedDataSize
                ? one
                : second;
        }
        
        internal static void OpenFileUsingDefaultProgram(string filePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };

            process.Start();
            process.WaitForExit();
        }

        internal static string ApplicationFilesPath
        {
            get
            {
                var baseDirectory = AppContext.BaseDirectory;

                if (string.IsNullOrWhiteSpace(baseDirectory) || baseDirectory == "/")
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return baseDirectory;
            }
        }
        
        internal static (float widthScale, float heightScale) CalculateSpaceScale(this SkSvgImage image, Size availableSpace)
        {
            var widthScale = CalculateDimensionScale(availableSpace.Width, image.Size.Width, image.Size.WidthUnit);
            var heightScale = CalculateDimensionScale(availableSpace.Height, image.Size.Height, image.Size.HeightUnit);

            return (widthScale, heightScale);
        
            float CalculateDimensionScale(float availableSize, float imageSize, SkSvgImageSize.Unit unit)
            {
                if (unit == Percentage)
                    return 100f / imageSize;

                if (unit is Centimeters or Millimeters or Inches or Points or Picas)
                    return availableSize / ConvertToPoints(imageSize, unit);   
            
                return availableSize / imageSize;
            }
        
            float ConvertToPoints(float value, SkSvgImageSize.Unit unit)
            {
                const float InchToCentimetre = 2.54f;
                const float InchToPoints = 72;
            
                // in CSS dpi is set to 96, but Skia uses more traditional 90
                const float PointToPixel = 90f / 72;
        
                var points =  unit switch
                {
                    Centimeters => value / InchToCentimetre * InchToPoints,
                    Millimeters => value / 10 / InchToCentimetre * InchToPoints,
                    Inches => value * InchToPoints,
                    Points => value,
                    Picas => value * 12,
                    _ => throw new ArgumentOutOfRangeException()
                };
        
                // different naming schema: SVG pixel = PDF point
                return points * PointToPixel;
            }
        }
    }
}