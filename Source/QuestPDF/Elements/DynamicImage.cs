using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements
{
    public class GenerateDynamicImageDelegatePayload
    {
        public Size AvailableSpace { get; set; }
        public ImageSize ImageSize { get; set; }
        public int Dpi { get; set; }
    }
    
    /// <summary>
    /// Generates an image based on the given resolution.
    /// </summary>
    /// <param name="size">Desired resolution of the image in pixels.</param>
    /// <param name="dpi">Desired resolution of the image in dots per inch.</param>
    /// <returns>An image in PNG, JPEG, or WEBP image format returned as byte array.</returns>
    public delegate byte[]? GenerateDynamicImageDelegate(GenerateDynamicImageDelegatePayload payload);
    
    internal sealed class DynamicImage : Element, IStateful
    {
        public bool IsRendered { get; set; }
        
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        public GenerateDynamicImageDelegate? Source { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            if (IsRendered)
                return SpacePlan.None();
            
            return SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (IsRendered)
                return;
            
            var dpi = TargetDpi ?? DocumentSettings.DefaultRasterDpi;
            
            var sourcePayload = new GenerateDynamicImageDelegatePayload
            {
                AvailableSpace = availableSpace,
                ImageSize = GetTargetResolution(availableSpace, dpi),
                Dpi = dpi
            };
            
            var imageBytes = Source?.Invoke(sourcePayload);
            
            if (imageBytes == null)
                return;

            using var imageData = SkData.FromBinary(imageBytes);
            using var originalImage = SkImage.FromData(imageData);
            
            if (UseOriginalImage)
            { 
                Canvas.DrawImage(originalImage, availableSpace);
                return;
            }
            
            using var compressedImage = originalImage.CompressImage(CompressionQuality.Value);

            var targetImage = Helpers.Helpers.GetImageWithSmallerSize(originalImage, compressedImage);
            Canvas.DrawImage(targetImage, availableSpace);
            
            IsRendered = true;
        }

        private static ImageSize GetTargetResolution(Size availableSize, int targetDpi)
        {
            var scalingFactor = targetDpi / (float)DocumentSettings.DefaultRasterDpi;

            return new ImageSize(
                (int)(availableSize.Width * scalingFactor),
                (int)(availableSize.Height * scalingFactor)
            );
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
