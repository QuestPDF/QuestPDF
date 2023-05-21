using System;
using System.Collections.Generic;
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
        internal bool IsDocumentScoped { get; }
        internal bool IsDisposed { get; private set; }
        
        internal LinkedList<(GetImageVersionRequest request, SKImage image)> ScaledImageCache { get; } = new();

        internal Image(SKImage image, bool isDocumentScoped)
        {
            SkImage = image;
            IsDocumentScoped = isDocumentScoped;
            Size = new ImageSize(image.Width, image.Height);
        }
        
        internal void Dispose()
        {
            if (IsDisposed)
                return;
            
            SkImage.Dispose();

            foreach (var cacheKey in ScaledImageCache)
                cacheKey.image.Dispose();

            IsDisposed = true;
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

            return new Image(image, true);
        }

        #endregion
    }

    public class SharedImage : Image, IDisposable
    {
        internal SharedImage(SKImage image) : base(image, false)
        {
            
        }
        
        public void Dispose()
        {
            base.Dispose();
        } 
        
        #region public constructors
        
        public static new SharedImage FromBinaryData(byte[] imageData)
        {
            return CreateImage(SKImage.FromEncodedData(imageData));
        }

        public static new SharedImage FromFile(string filePath)
        {
            return CreateImage(SKImage.FromEncodedData(filePath));
        }

        public static new SharedImage FromStream(Stream fileStream)
        {
            return CreateImage(SKImage.FromEncodedData(fileStream));
        }

        private static SharedImage CreateImage(SKImage? image)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");

            var result = new SharedImage(image);
            
            return result;
        }

        #endregion
    }
}