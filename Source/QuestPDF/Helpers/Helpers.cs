using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QuestPDF.Drawing;
using QuestPDF.Drawing.DrawingCanvases;
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
        
        internal static string PrettifyName(this string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled);
        }

        internal static void VisitChildren(this Element? root, Action<Element?> handler)
        {
            Traverse(root);

            void Traverse(Element? element)
            {
                if (element == null)
                    return;
                
                if (element is ContainerElement containerElement)
                {
                    Traverse(containerElement.Child);
                }
                else
                {
                    foreach (var child in element.GetChildren())
                        Traverse(child);  
                }
            
                handler(element);
            }
        }

        internal static void ReleaseDisposableChildren(this Element? element)
        {
            element.VisitChildren(x => (x as IDisposable)?.Dispose());
        }
        
        internal static bool IsGreaterThan(this float first, float second)
        {
            return first > second + Size.Epsilon;
        }
        
        internal static bool IsLessThan(this float first, float second)
        {
            return first < second - Size.Epsilon;
        }
        
        public static bool AreClose(double a, double b)
        {
            return Math.Abs(a - b) <= Size.Epsilon;
        }

        internal static bool IsNegative(this Size size)
        {
            return size.Width < -Size.Epsilon || size.Height < -Size.Epsilon;
        }
        
        internal static bool IsCloseToZero(this Size size)
        {
            return Math.Abs(size.Width) < Size.Epsilon && Math.Abs(size.Height) < Size.Epsilon;
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
            if (targetResolution.Width == 0 || targetResolution.Height == 0)
                targetResolution = new ImageSize(1, 1);
            
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
                    return 1;

                if (unit is Centimeters or Millimeters or Inches or Points or Picas)
                    return availableSize / ConvertToPoints(imageSize, unit);   
            
                return availableSize / imageSize;
            }
        
            float ConvertToPoints(float value, SkSvgImageSize.Unit unit)
            {
                const float inchToCentimetre = 2.54f;
                const float inchToPoints = 72;
        
                var points =  unit switch
                {
                    Centimeters => value / inchToCentimetre * inchToPoints,
                    Millimeters => value / 10 / inchToCentimetre * inchToPoints,
                    Inches => value * inchToPoints,
                    Points => value,
                    Picas => value * 12,
                    _ => throw new ArgumentOutOfRangeException()
                };
        
                // different naming schema: SVG pixel = PDF point
                return points * GetScalingFactor();
            }

            float GetScalingFactor()
            {
                // in CSS dpi is set to 96, but Skia uses more traditional 90
                // when the SVG ViewBox attribute is present, Skia uses legacy/traditional 90 DPI
                // otherwise, we should assume modern CSS-based 96 DPI for better compatibility
                var targetDpi = HasViewBox() ? 90f : 96f;
                return targetDpi / 72;
            }

            bool HasViewBox()
            {
                return image.ViewBox is not
                {
                    Left : 0f, 
                    Top : 0f, 
                    Width : 0f, 
                    Height : 0f
                };
            }
        }

        public static string FormatAsCompanionNumber(this float value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        
        public static bool Is<T>(this IDrawingCanvas canvas) where T : IDrawingCanvas
        {
            var canvasUnderTest = canvas;

            while (canvasUnderTest is ProxyDrawingCanvas proxy)
                canvasUnderTest = proxy.Target;

            return canvasUnderTest is T;
        }
    }
}