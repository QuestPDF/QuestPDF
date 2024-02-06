using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Helpers
{
    internal static class Helpers
    {
        static Helpers()
        {
            NativeDependencyCompatibilityChecker.Test();
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
        
        internal static SKEncodedImageFormat ToSkImageFormat(this ImageFormat format)
        {
            return format switch
            {
                ImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
                ImageFormat.Png => SKEncodedImageFormat.Png,
                ImageFormat.Webp=> SKEncodedImageFormat.Webp,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
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
        
        internal static SKImage ScaleImage(this SKImage image, ImageSize targetResolution)
        {
            var imageInfo = new SKImageInfo(targetResolution.Width, targetResolution.Height, image.Info.ColorType, image.Info.AlphaType, image.Info.ColorSpace);
            
            using var bitmap = SKBitmap.FromImage(image);
            using var resultBitmap = bitmap.Resize(imageInfo, SKFilterQuality.Medium);
            return SKImage.FromBitmap(resultBitmap);
        }
        
        internal static SKImage CompressImage(this SKImage image, ImageCompressionQuality compressionQuality)
        {
            var targetFormat = image.Info.IsOpaque 
                ? SKEncodedImageFormat.Jpeg 
                : SKEncodedImageFormat.Png;

            if (targetFormat == SKEncodedImageFormat.Png)
                compressionQuality = ImageCompressionQuality.Best;
            
            var data = image.Encode(targetFormat, compressionQuality.ToQualityValue());
            return SKImage.FromEncodedData(data);
        }

        internal static SKImage ResizeAndCompressImage(this SKImage image, ImageSize targetResolution, ImageCompressionQuality compressionQuality)
        {
            if (image.Width == targetResolution.Width && image.Height == targetResolution.Height)
                return CompressImage(image, compressionQuality);
            
            using var scaledImage = image.ScaleImage(targetResolution);
            return CompressImage(scaledImage, compressionQuality);
        }

        internal static SKImage GetImageWithSmallerSize(SKImage one, SKImage second)
        {
            return one.EncodedData.Size < second.EncodedData.Size
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
    }
}