using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Fluent
{
    public static class ImageExtensions
    {
        public static ImageDescriptor Image(this IContainer parent, byte[] imageData)
        {
            var image = Infrastructure.Image.FromBinaryData(imageData).DisposeAfterDocumentGeneration();
            return parent.Image(image);
        }
        
        public static ImageDescriptor Image(this IContainer parent, string filePath)
        {
            var image = Infrastructure.Image.FromFile(filePath).DisposeAfterDocumentGeneration();
            return parent.Image(image);
        }
        
        public static ImageDescriptor Image(this IContainer parent, Stream fileStream)
        {
            var image = Infrastructure.Image.FromStream(fileStream).DisposeAfterDocumentGeneration();
            return parent.Image(image);
        }
        
        internal static ImageDescriptor Image(this IContainer parent, Infrastructure.Image image)
        {
            if (image == null)
                throw new DocumentComposeException("Cannot load or decode provided image.");
            
            var imageElement = new QuestPDF.Elements.Image
            {
                DocumentImage = image
            };

            var aspectRationElement = new AspectRatio
            {
                Child = imageElement
            };
            
            parent.Element(aspectRationElement);
            return new ImageDescriptor(imageElement, aspectRationElement).FitWidth();
        }

        public static void Image(this IContainer element, GenerateDynamicImageDelegate dynamicImageSource)
        {
            element.Element(new DynamicImage
            {
                Source = dynamicImageSource
            });
        }
        
        #region Obsolete
        
        [Obsolete("This element has been changed since version 2023.5. Please use the Image method overload that takes the GenerateDynamicImageDelegate as an argument.")]
        public static void Image(this IContainer element, Func<Size, byte[]> imageSource)
        {
            element.Image((ImageSize x) => imageSource(new Size(x.Width, x.Height)));
        }
        
        [Obsolete("This element has been changed since version 2023.5. Please use the Image method overload that returns the ImageDescriptor object.")]
        public static void Image(this IContainer parent, byte[] imageData, ImageScaling scaling)
        {
            parent.Image(imageData).ApplyScaling(scaling);
        }
        
        [Obsolete("This element has been changed since version 2023.5. Please use the Image method overload that returns the ImageDescriptor object.")]
        public static void Image(this IContainer parent, string filePath, ImageScaling scaling)
        {
            parent.Image(filePath).ApplyScaling(scaling);
        }
        
        [Obsolete("This element has been changed since version 2023.5. Please use the Image method overload that returns the ImageDescriptor object.")]
        public static void Image(this IContainer parent, Stream fileStream, ImageScaling scaling)
        {
            parent.Image(fileStream).ApplyScaling(scaling);
        }
        
        internal static void ApplyScaling(this ImageDescriptor descriptor, ImageScaling scaling)
        {
            if (scaling == ImageScaling.Resize)
                descriptor.FitUnproportionally();

            else if (scaling == ImageScaling.FitWidth)
                descriptor.FitWidth();
            
            else if (scaling == ImageScaling.FitHeight)
                descriptor.FitHeight();
            
            else if (scaling == ImageScaling.FitArea)
                descriptor.FitArea();
        }
        
        #endregion
    }
}