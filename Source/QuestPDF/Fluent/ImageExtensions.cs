using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Fluent
{
    public static class ImageExtensions
    {
        public static void Image(this IContainer parent, byte[] imageData, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(imageData);
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, string filePath, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(filePath);
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, Stream fileStream, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = SKImage.FromEncodedData(fileStream);
            parent.Image(image, scaling);
        }
        
        private static void Image(this IContainer parent, SKImage image, ImageScaling scaling = ImageScaling.FitWidth)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");
            
            var imageElement = new Image
            {
                InternalImage = image
            };

            if (scaling != ImageScaling.Resize)
            {
                var aspectRatio = image.Width / (float)image.Height;
                parent = parent.AspectRatio(aspectRatio, Map(scaling));
            }

            parent.Element(imageElement);

            static AspectRatioOption Map(ImageScaling scaling)
            {
                return scaling switch
                {
                    ImageScaling.FitWidth => AspectRatioOption.FitWidth,
                    ImageScaling.FitHeight => AspectRatioOption.FitHeight,
                    ImageScaling.FitArea => AspectRatioOption.FitArea,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public static void Image(this IContainer element, Func<Size, byte[]> imageSource)
        {
            element.Element(new DynamicImage
            {
                Source = imageSource
            });
        }
    }
}