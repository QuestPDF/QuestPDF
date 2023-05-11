using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element, ICacheable
    {
        public Infrastructure.Image? DocumentImage { get; set; }

        internal bool UseOriginalImage { get; set; }
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }

        ~Image()
        {
            if (DocumentImage is { IsDocumentScoped: true })
                DocumentImage?.Dispose();
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (DocumentImage == null)
                return;

            var image = GetImageToDraw(availableSpace);
            Canvas.DrawImage(image, Position.Zero, availableSpace);
        }

        private SKImage GetImageToDraw(Size availableSpace)
        {
            var originalImage = DocumentImage.SkImage;
            
            if (UseOriginalImage)
                return originalImage;
            
            var request = new GetImageVersionRequest
            {
                Resolution = GetTargetResolution(DocumentImage.Size, availableSpace, TargetDpi.Value),
                CompressionQuality = CompressionQuality.Value
            };
            
            var targetImage = DocumentImage.GetVersionOfSize(request);
            return Helpers.Helpers.GetImageWithSmallerSize(originalImage, targetImage);
        }
        
        private static ImageSize GetTargetResolution(ImageSize imageResolution, Size availableAreaSize, int targetDpi)
        {
            var scalingFactor = targetDpi / (float)DocumentSettings.DefaultRasterDpi;
            
            var targetResolution = new ImageSize(
                (int)(availableAreaSize.Width * scalingFactor), 
                (int)(availableAreaSize.Height * scalingFactor));
            
            var isImageResolutionSmallerThanTarget = imageResolution.Width < targetResolution.Width || imageResolution.Height < targetResolution.Height;

            if (isImageResolutionSmallerThanTarget)
                return imageResolution;

            return targetResolution;
        }
    }
}