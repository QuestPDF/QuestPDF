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
            var image = Infrastructure.Image.FromBinaryData(imageData).DisposeAfterDocumentGeneration();
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, string filePath, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = Infrastructure.Image.FromFile(filePath).DisposeAfterDocumentGeneration();
            parent.Image(image, scaling);
        }
        
        public static void Image(this IContainer parent, Stream fileStream, ImageScaling scaling = ImageScaling.FitWidth)
        {
            var image = Infrastructure.Image.FromStream(fileStream).DisposeAfterDocumentGeneration();
            parent.Image(image, scaling);
        }
        
        internal static void Image(this IContainer parent, Infrastructure.Image image, ImageScaling scaling = ImageScaling.FitWidth)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");
            
            var imageElement = new QuestPDF.Elements.Image
            {
                DocumentImage = image
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