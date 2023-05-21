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