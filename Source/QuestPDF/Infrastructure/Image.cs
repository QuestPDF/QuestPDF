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
        internal int? ImageQuality { get; set; }
        internal bool PerformScalingToTargetDpi { get; set; }
        internal bool IsDocumentScoped { get; set; }

        public int Width => SkImage.Width;
        public int Height => SkImage.Height;

        private const float ImageSizeSimilarityToleranceMax = 1.1f;
        private const float ImageSizeSimilarityToleranceMin = 1 / ImageSizeSimilarityToleranceMax;

        private Image(SKImage image)
        {
            SkImage = image;
            PerformScalingToTargetDpi = image.EncodedData.Size >= Settings.AdjustImageSizeThreshold;
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

            var scaledImage = ScaleImage(SkImage, targetResolution, ImageQuality);
            ScaledImageCache.Add((targetResolution, scaledImage));

            if (SkImage.EncodedData.Size < scaledImage.EncodedData.Size)
                return SkImage;

            return scaledImage;

            
            
            static SKImage ScaleImage(SKImage originalImage, Size targetSize, int? imageQuality)
            {
                var imageInfo = new SKImageInfo((int)targetSize.Width, (int)targetSize.Height);
                using var target = SKImage.Create(imageInfo);
                originalImage.ScalePixels(target.PeekPixels(), SKFilterQuality.High);

                var codes = SKCodec.Create(target.EncodedData);

                var targetFormat = imageQuality > 100 ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
                var targetQuality = Math.Max(imageQuality, 100);
                var data = target.Encode(targetFormat, targetQuality);

                return SKImage.FromEncodedData(data);
            }

            static (SKEncodedImageFormat format, int quality) GetTargetImageFormat(SKImage originalImage, int? imageQuality) 
            {
                if (imageQuality.HasValue)
                {
                    var format = imageQuality > 100 
                        ? SKEncodedImageFormat.Png 
                        : SKEncodedImageFormat.Jpeg;

                    var quality = Math.Max(imageQuality.Value, 100);

                    return (format, quality);
                }
                
                var codec = SKCodec.Create(originalImage.EncodedData);
                
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

        /// <summary>
        /// Values from 1 to 100 correspond to the JPEG format, where 1 is lowest and 100 is highest quality.
        /// Value 101 correspond to the PNG format with a lossless compression and alpha channel support. 
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public Image WithQuality(int quality)
        {
            ImageQuality = quality;
            return this;
        }
        
        public Image WithQuality(ImageQuality quality)
        {
            ImageQuality = (int)quality;
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