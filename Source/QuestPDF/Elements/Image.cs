using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    internal sealed class Image : Element, IStateful, ICacheable
    {
        public bool IsRendered { get; set; }
        public Infrastructure.Image? DocumentImage { get; set; }

        internal bool UseOriginalImage { get; set; }
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            if (IsRendered)
                return SpacePlan.None();
            
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
    
        object IStateful.CloneState()
        {
            return IsRendered;
        }

        void IStateful.SetState(object state)
        {
            IsRendered = (bool) state;
        }

        void IStateful.ResetState(bool hardReset)
        {
            IsRendered = false;
        }
    
        #endregion
    }
}