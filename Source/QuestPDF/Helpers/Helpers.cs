using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

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
        
        internal static SkImage CompressImage(this SkImage image, ImageCompressionQuality compressionQuality)
        {
            return image.ResizeAndCompress(image.Width, image.Height, compressionQuality.ToQualityValue());
        }

        internal static SkImage ResizeAndCompressImage(this SkImage image, ImageSize targetResolution, ImageCompressionQuality compressionQuality)
        {
            return image.ResizeAndCompress(targetResolution.Width, targetResolution.Height, compressionQuality.ToQualityValue());
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
    }
}