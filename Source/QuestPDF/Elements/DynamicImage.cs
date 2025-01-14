using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        public GenerateDynamicImageDelegate? Source { get; set; }
        
        private List<(Size Size, SkImage? Image)> Cache { get; } = new(1);

        private float GenerationTime { get; set; }
        private int DrawnImageSize { get; set; }
        
        ~DynamicImage()
        {
            foreach (var cacheItem in Cache)
                cacheItem.Image?.Dispose();
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();

            if (availableSpace.IsNegative())
                return SpacePlan.Wrap("The available space is negative.");
        
            return SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var stopWatch = Stopwatch.StartNew();
            
            var targetImage = Cache.FirstOrDefault(x => Size.Equal(x.Size, availableSpace)).Image;
            
            if (targetImage == null)
            {
                targetImage = GetImage(availableSpace);
                Cache.Add((availableSpace, targetImage));
            }
       
            if (targetImage != null)
                Canvas.DrawImage(targetImage, availableSpace);
            
            targetImage?.Dispose();
            
            GenerationTime += (float) stopWatch.Elapsed.TotalMilliseconds;
            DrawnImageSize += targetImage?.EncodedDataSize ?? 0;
            
            IsRendered = true;
        }

        private SkImage? GetImage(Size availableSpace)
        {
            var dpi = TargetDpi ?? DocumentSettings.DefaultRasterDpi;
            
            var sourcePayload = new GenerateDynamicImageDelegatePayload
            {
                AvailableSpace = availableSpace,
                ImageSize = GetTargetResolution(availableSpace, dpi),
                Dpi = dpi
            };
            
            var imageBytes = Source?.Invoke(sourcePayload);
            
            if (imageBytes == null)
                return null;

            using var imageData = SkData.FromBinary(imageBytes);
            var originalImage = SkImage.FromData(imageData);

            if (UseOriginalImage)
                return originalImage;
            
            var compressedImage = originalImage.CompressImage(CompressionQuality.Value);

            if (originalImage.EncodedDataSize > compressedImage.EncodedDataSize)
            {
                originalImage.Dispose();
                return compressedImage;
            }
            else
            {
                compressedImage.Dispose();
                return originalImage;
            }
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
        
        private bool IsRendered { get; set; }
    
        public void ResetState(bool hardReset = false) => IsRendered = false;
        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion
        
        internal override string? GetCompanionHint()
        {
            var sizeKB = Math.Max(1, DrawnImageSize / 1024);
            return $"{sizeKB}KB, generated in {GenerationTime:0.00}ms";
        }
    }
}
