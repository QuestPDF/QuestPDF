using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Fluent
{
    public static class ImageExtensions
    {
        public static void Image(this IContainer parent, byte[] data, ImageScaling scaling = ImageScaling.FitWidth)
        {
            if (data == null)
                return;
            
            var image = SKImage.FromEncodedData(data);
            var aspectRatio = image.Width / (float)image.Height;
            
            var imageElement = new Image
            {
                InternalImage = image
            };

            if (scaling != ImageScaling.Resize)
                parent = parent.AspectRatio(aspectRatio, Map(scaling));
            
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

        public static void DynamicImage(this IContainer element, Func<Size, byte[]> imageSource)
        {
            element.Element(new DynamicImage
            {
                Source = imageSource
            });
        }
    }
}