using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal sealed class Image : Element, IStateful, IDisposable
    {
        public Infrastructure.Image? DocumentImage { get; set; }

        internal bool UseOriginalImage { get; set; }
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
 
        private int DrawnImageSize { get; set; }

        ~Image()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            if (DocumentImage != null && !DocumentImage.IsShared) 
                DocumentImage?.Dispose();
            
            GC.SuppressFinalize(this);
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();

            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");
        
            return SpacePlan.FullRender(Size.Zero);
        }

        internal override void Draw(Size availableSpace)
        {
            if (DocumentImage == null)
                return;
            
            if (IsRendered)
                return;

            var image = GetImageToDraw(availableSpace);
            Canvas.DrawImage(image, availableSpace);

            DrawnImageSize = Math.Max(DrawnImageSize, image.EncodedDataSize);
            
            IsRendered = true;
        }

        private SkImage GetImageToDraw(Size availableSpace)
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
        
        #region IStateful
        
        private bool IsRendered { get; set; }
    
        public void ResetState(bool hardReset = false) => IsRendered = false;
        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion

        internal override string? GetCompanionHint()
        {
            var sizeKB = Math.Max(1, DrawnImageSize / 1024);
            return $"{sizeKB}KB";
        }
    }
}