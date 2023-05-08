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
        internal ImageResizeStrategy? ResizeStrategy { get; set; }
        
        private const float ImageSizeSimilarityToleranceMax = 0.75f;
        
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
            if (UseOriginalImage)
                return DocumentImage.SkImage;
            
            var request = new GetImageVersionRequest
            {
                Resolution = GetTargetResolution(ResizeStrategy.Value, DocumentImage.Size, availableSpace, TargetDpi.Value),
                CompressionQuality = CompressionQuality.Value,
                ResizeStrategy = ResizeStrategy.Value
            };
            
            return DocumentImage.GetVersionOfSize(request);
        }
        
        private static ImageSize GetTargetResolution(ImageResizeStrategy resizeStrategy, ImageSize imageResolution, Size availableAreaSize, int targetDpi)
        {
            if (resizeStrategy == ImageResizeStrategy.Never)
                return imageResolution;

            var scalingFactor = targetDpi / (float)DocumentSettings.DefaultRasterDpi;
            var targetResolution = new ImageSize(
                (int)(availableAreaSize.Width * scalingFactor), 
                (int)(availableAreaSize.Height * scalingFactor));

            if (resizeStrategy == ImageResizeStrategy.Always)
                return targetResolution;
            
            var isSignificantlySmaller = IsImageSignificantlySmallerThanDrawingArea(imageResolution, targetResolution);
            
            if (resizeStrategy == ImageResizeStrategy.ScaleOnlyToSignificantlySmallerResolution && isSignificantlySmaller)
                return targetResolution;

            var isSmaller = IsImageSmallerThanDrawingArea(imageResolution, targetResolution);

            if (resizeStrategy == ImageResizeStrategy.ScaleOnlyToSmallerResolution && isSmaller)
                return targetResolution;

            return imageResolution;
        }
        
        private static bool IsImageSmallerThanDrawingArea(ImageSize imageResolution, ImageSize targetResolution)
        {
            return imageResolution.Width < targetResolution.Width || imageResolution.Height < targetResolution.Height;
        }
        
        private static bool IsImageSignificantlySmallerThanDrawingArea(ImageSize imageResolution, ImageSize targetResolution, float sizeSimilarityThreshold = ImageSizeSimilarityToleranceMax)
        {
            var widthRatio = targetResolution.Width / imageResolution.Width;
            var heightRatio = targetResolution.Height / imageResolution.Height;
        
            return widthRatio < sizeSimilarityThreshold && heightRatio < sizeSimilarityThreshold;
        }
    }
}