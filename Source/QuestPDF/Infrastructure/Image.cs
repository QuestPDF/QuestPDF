using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class Image : IDisposable
    {
        private SKImage SkImage { get; }
        public int Width => SkImage.Width;
        public int Height => SkImage.Height;

        internal List<(Size size, SKImage image)>? ScaledImageCache { get; set; }
        
        internal bool IsDocumentScoped { get; set; }
        internal bool UseOriginalImage { get; set; }
        internal int? TargetDpi { get; set; }
        internal ImageCompressionQuality? CompressionQuality { get; set; }
        internal ImageScalingQuality? ScalingQuality { get; set; }
        internal ImageScalingStrategy? ScalingStrategy { get; set; }

        private const float ImageSizeSimilarityToleranceMax = 0.75f;
        
        private Image(SKImage image)
        {
            SkImage = image;
        }

        public void Dispose()
        {
            SkImage.Dispose();
        } 
        
        #region Scaling Image

        internal SKImage GetVersionOfSize(Size availableAreaSize)
        {
            if (UseOriginalImage)
                return SkImage;

            var imageResolution = new Size(SkImage.Width, SkImage.Height);
            var targetResolution = GetTargetResolution(ScalingStrategy.Value, imageResolution, availableAreaSize, TargetDpi ?? DocumentSettings.DefaultRasterDpi);
            
            return ScaleAndCompressImage(SkImage, targetResolution, ScalingQuality.Value, CompressionQuality.Value);
        }
        
        private static Size GetTargetResolution(ImageScalingStrategy scalingStrategy, Size imageResolution, Size availableAreaSize, int targetDpi)
        {
            if (scalingStrategy == ImageScalingStrategy.Never)
                return imageResolution;
            
            if (scalingStrategy == ImageScalingStrategy.Always)
                return availableAreaSize;
            
            var scalingFactor = targetDpi / DocumentSettings.DefaultRasterDpi;
            var targetResolution = new Size(availableAreaSize.Width * scalingFactor, availableAreaSize.Height * scalingFactor);

            var isSignificantlySmaller = IsImageSignificantlySmallerThanDrawingArea(imageResolution, targetResolution);
            
            if (scalingStrategy == ImageScalingStrategy.ScaleOnlyToSignificantlySmallerResolution && isSignificantlySmaller)
                return targetResolution;

            var isSmaller = IsImageSmallerThanDrawingArea(imageResolution, targetResolution);

            if (scalingStrategy == ImageScalingStrategy.ScaleOnlyToSmallerResolution && isSmaller)
                return targetResolution;

            return imageResolution;
        }
        
        private static SKImage CompressImage(SKImage image, ImageCompressionQuality compressionQuality)
        {
            var targetFormat = image.Info.IsOpaque 
                ? SKEncodedImageFormat.Png 
                : SKEncodedImageFormat.Jpeg;

            if (targetFormat == SKEncodedImageFormat.Png)
                compressionQuality = ImageCompressionQuality.Best;
            
            var data = image.Encode(targetFormat, compressionQuality.ToQualityValue());
            return SKImage.FromEncodedData(data);
        }

        private static SKImage ScaleAndCompressImage(SKImage image, Size targetResolution, ImageScalingQuality scalingQuality, ImageCompressionQuality compressionQuality)
        {
            var imageInfo = new SKImageInfo((int)targetResolution.Width, (int)targetResolution.Height);
            
            using var resultImage = SKImage.Create(imageInfo);
            image.ScalePixels(resultImage.PeekPixels(), scalingQuality.ToFilterQuality());
            
            return CompressImage(resultImage, compressionQuality);
        }
        
        private static bool IsImageSmallerThanDrawingArea(Size imageResolution, Size targetResolution)
        {
            return imageResolution.Width < targetResolution.Width || imageResolution.Height < targetResolution.Height;
        }
        
        private static bool IsImageSignificantlySmallerThanDrawingArea(Size imageResolution, Size targetResolution, float sizeSimilarityThreshold = ImageSizeSimilarityToleranceMax)
        {
            var widthRatio = imageResolution.Width / targetResolution.Width;
            var heightRatio = imageResolution.Height / targetResolution.Height;
        
            return widthRatio < sizeSimilarityThreshold && heightRatio < sizeSimilarityThreshold;
        }
        
        #endregion

        #region public constructors

        internal static Image FromSkImage(SKImage image)
        {
            return CreateImage(image);
        }

        public static Image FromBinaryData(byte[] imageData)
        {
            return CreateImage(SKImage.FromEncodedData(imageData));
        }

        public static Image FromFile(string filePath)
        {
            return CreateImage(SKImage.FromEncodedData(filePath));
        }

        public static Image FromStream(Stream fileStream)
        {
            return CreateImage(SKImage.FromEncodedData(fileStream));
        }

        private static Image CreateImage(SKImage? image)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");

            return new Image(image);
        }

        #endregion
    }
}