using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    internal record GetImageVersionRequest
    {
        internal ImageSize Resolution { get; set; }
        internal ImageCompressionQuality CompressionQuality { get; set; }
    }
    
    public class Image
    {
        internal SKImage SkImage { get; }
        internal ImageSize Size { get; }

        internal LinkedList<(GetImageVersionRequest request, SKImage image)> ScaledImageCache { get; } = new();
 
        internal Image(SKImage image)
        {
            SkImage = image;
            Size = new ImageSize(image.Width, image.Height);
        }
        
        ~Image()
        {
            SkImage.Dispose();
            
            foreach (var cacheKey in ScaledImageCache)
                cacheKey.image.Dispose();
        }
        
        #region Scaling Image

        internal SKImage GetVersionOfSize(GetImageVersionRequest request)
        {
            foreach (var cacheKey in ScaledImageCache)
            {
                if (cacheKey.request == request)
                    return cacheKey.image;
            }

            var result = SkImage.ResizeAndCompressImage(request.Resolution, request.CompressionQuality);
            ScaledImageCache.AddLast((request, result));
            return result;
        }

        #endregion

        #region public constructors

        private const string CannotDecodeExceptionMessage = "Cannot decode the provided image.";
        
        internal static Image FromSkImage(SKImage image)
        {
            return new Image(image);
        }

        public static Image FromBinaryData(byte[] imageData)
        {
            var image = SKImage.FromEncodedData(imageData);
            
            if (image == null)
                throw new DocumentComposeException(CannotDecodeExceptionMessage);
            
            return new Image(image);
        }

        public static Image FromFile(string filePath)
        {
            var image = SKImage.FromEncodedData(filePath);

            if (image == null)
            {
                throw File.Exists(filePath) 
                    ? new DocumentComposeException(CannotDecodeExceptionMessage)
                    : new DocumentComposeException($"Cannot load provided image, file not found: ${filePath}");
            }
            
            return new Image(image);
        }

        public static Image FromStream(Stream fileStream)
        {
            var image = SKImage.FromEncodedData(fileStream);
            
            if (image == null)
                throw new DocumentComposeException(CannotDecodeExceptionMessage);
            
            return new Image(image);
        }

        #endregion
    }
}