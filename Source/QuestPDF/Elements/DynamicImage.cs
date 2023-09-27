using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    /// <summary>
    /// Generates an image based on the given resolution.
    /// </summary>
    /// <param name="size">Desired resolution of the image in pixels.</param>
    /// <returns>An image in PNG, JPEG, or WEBP image format returned as byte array.</returns>
    public delegate byte[] GenerateDynamicImageDelegate(ImageSize size);

    internal sealed class DynamicImage : Element
    {
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        public GenerateDynamicImageDelegate? Source { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var targetResolution = GetTargetResolution(availableSpace, TargetDpi.Value);
            var imageData = Source?.Invoke(targetResolution);
            
            if (imageData == null)
                return;

            using var originalImage = SKImage.FromEncodedData(imageData);
            
            if (UseOriginalImage)
            {
                Canvas.DrawImage(originalImage, Position.Zero, availableSpace);
                return;
            }
            
            using var compressedImage = originalImage.CompressImage(CompressionQuality.Value);

            var targetImage = Helpers.Helpers.GetImageWithSmallerSize(originalImage, compressedImage);
            Canvas.DrawImage(targetImage, Position.Zero, availableSpace);
        }

        private static ImageSize GetTargetResolution(Size availableSize, int targetDpi)
        {
            var scalingFactor = targetDpi / (float)DocumentSettings.DefaultRasterDpi;

            return new ImageSize(
                (int)(availableSize.Width * scalingFactor),
                (int)(availableSize.Height * scalingFactor)
            );
        }
    }
}
