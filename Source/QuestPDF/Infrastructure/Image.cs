using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class Image : IDisposable
    {
        private SKImage SkImage { get; }
        internal List<(Size size, SKImage image)>? ScaledImageCache { get; }
        internal bool IsDocumentScoped { get; set; }
        
        public int Width => SkImage.Width;
        public int Height => SkImage.Height;
        
        private Image(SKImage image)
        {
            SkImage = image;
        }

        public Image DisposeAfterDocumentGeneration()
        {
            IsDocumentScoped = true;
            return this;
        }
        
        public void Dispose()
        {
            SkImage.Dispose();
            ScaledImageCache?.ForEach(x => x.image.Dispose());
        }

        internal SKImage GetVersionOfSize(Size size)
        {
            if (size.Width > Width || size.Height > Height)
                return SkImage;
            
            var target = SKImage.Create(new SKImageInfo((int)size.Width, (int)size.Height));
            SkImage.ScalePixels(target.PeekPixels(), SKFilterQuality.High);

            return target;

            // bool ShouldApplyScaling()
            // {
            //     const float tolerance = 1.1f;
            //     
            //     if (width > Width / tolerance || height > Height / tolerance)
            //         return false;
            //     
            //     SkImage.sca
            // }
            
            static bool HasSimilarSize(Size a, Size b, float tolerance)
            {
                return (a.Width / b.Width > 1 / tolerance && a.Width / b.Width < tolerance) ||
                       (a.Height / b.Height > 1 / tolerance && a.Height / b.Height < tolerance);
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
    }
}