using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class Image : IDisposable
    {
        private SKImage SkImage { get; }
        internal List<(Size size, SKImage image)>? ScaledImageCache { get; set; }
        
        internal int? TargetDpi { get; set; }
        internal bool PerformScalingToTargetDpi { get; set; } = true;
        internal bool IsDocumentScoped { get; set; }
        
        public int Width => SkImage.Width;
        public int Height => SkImage.Height;

        private const float ImageSizeSimilarityToleranceMax = 1.1f;
        private const float ImageSizeSimilarityToleranceMin = 1 / ImageSizeSimilarityToleranceMax;
        
        private Image(SKImage image)
        {
            SkImage = image;
        }

        public void Dispose()
        {
            SkImage.Dispose();
            ScaledImageCache?.ForEach(x => x.image.Dispose());
        }

        internal SKImage GetVersionOfSize(Size size)
        {
            if (!PerformScalingToTargetDpi)
                return SkImage;

            var scalingFactor = TargetDpi.Value / (float)DocumentMetadata.DefaultPdfDpi;
            var targetResolution = new Size(size.Width * scalingFactor, size.Height * scalingFactor);
            
            if (targetResolution.Width > Width || targetResolution.Height > Height)
                return SkImage;
            
            ScaledImageCache ??= new List<(Size size, SKImage image)>();
            
            foreach (var imageCache in ScaledImageCache)
            {
                if (HasSimilarSize(imageCache.size, targetResolution))
                    return imageCache.image;
            }

            var scaledImage = ScaleImage(SkImage, targetResolution);
            ScaledImageCache.Add((targetResolution, scaledImage));

            return scaledImage;
     
            static SKImage ScaleImage(SKImage originalImage, Size targetSize)
            {
                var imageInfo = new SKImageInfo((int)targetSize.Width, (int)targetSize.Height);
                var target = SKImage.Create(imageInfo);
                originalImage.ScalePixels(target.PeekPixels(), SKFilterQuality.High);

                return target;
            }
            
            static bool HasSimilarSize(Size a, Size b)
            {
                var widthRatio = a.Width / b.Width;
                var heightRatio = a.Height / b.Height;

                return widthRatio is > ImageSizeSimilarityToleranceMin and < ImageSizeSimilarityToleranceMax &&
                       heightRatio is > ImageSizeSimilarityToleranceMin and < ImageSizeSimilarityToleranceMax;
            }
        }

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
        
        #region configuration API
        
        public Image DisposeAfterDocumentGeneration()
        {
            IsDocumentScoped = true;
            return this;
        }

        public Image WithTargetDpi(int dpi = DocumentMetadata.DefaultPdfDpi)
        {
            TargetDpi = dpi;
            return this;
        }

        public Image ScaleToTargetDpi(bool value = true)
        {
            PerformScalingToTargetDpi = value;
            return this;
        }

        #endregion
    }
}