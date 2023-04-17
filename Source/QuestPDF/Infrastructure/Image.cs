using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    public class Image : IDisposable
    {
        internal SKImage SkImage { get; }
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