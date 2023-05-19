using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Fluent
{
    public class ImageDescriptor
    {
        private Elements.Image ImageElement { get; }
        private AspectRatio AspectRatioElement { get; }
        private float ImageAspectRatio { get; }

        internal ImageDescriptor(Elements.Image imageElement, Elements.AspectRatio aspectRatioElement)
        {
            ImageElement = imageElement;
            AspectRatioElement = aspectRatioElement;

            var imageSize = ImageElement.DocumentImage.Size;
            ImageAspectRatio = imageSize.Width / (float)imageSize.Height;
        }
        
        /// <summary>
        /// When enabled, the library will not attempt to resize the image to fit the target DPI, nor save it with target image quality.
        /// </summary>
        public ImageDescriptor UseOriginalImage(bool value = true)
        {
            ImageElement.UseOriginalImage = value;
            return this;
        }
        
        /// <summary>
        /// The DPI (pixels-per-inch) at which images and features without native PDF support will be rasterized.
        /// A larger DPI would create a PDF that reflects the original intent with better fidelity, but it can make for larger PDF files too, which would use more memory while rendering, and it would be slower to be processed or sent online or to printer.
        /// When generating images, this parameter also controls the resolution of the generated content.
        /// Default value is 144.
        /// </summary>
        public ImageDescriptor WithRasterDpi(int dpi)
        {
            ImageElement.TargetDpi = dpi;
            return this;
        }

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// When the image is opaque, it will be encoded using the JPEG format with the selected quality setting.
        /// When the image contains an alpha channel, it is always encoded using the PNG format and this option is ignored.
        /// The default value is "very high quality".
        /// </summary>
        public ImageDescriptor WithCompressionQuality(ImageCompressionQuality quality)
        {
            ImageElement.CompressionQuality = quality;
            return this;
        }

        #region Aspect Ratio
        
        /// <summary>
        /// The image scales to take the entire available width. Default.
        /// </summary>
        public ImageDescriptor FitWidth()
        {
            return SetAspectRatio(AspectRatioOption.FitWidth);
        }
        
        /// <summary>
        /// The images scales to take the entire available height. Good in conjunction with constraining elements.
        /// </summary>
        public ImageDescriptor FitHeight()
        {
            return SetAspectRatio(AspectRatioOption.FitHeight);
        }
        
        /// <summary>
        /// This is the combination of both of the FitWidth and the FitHeight options. 
        /// The element scales to occupy the entire available area while preserving its aspect ratio.
        /// This means that sometimes it occupies the entire width and sometimes the entire height.
        /// This is the safest option.
        /// </summary>
        public ImageDescriptor FitArea()
        {
            return SetAspectRatio(AspectRatioOption.FitArea);
        }
        
        /// <summary>
        /// The image resizes itself to occupy the entire available space.
        /// It does not preserve proportions.
        /// The image may look incorrectly scaled, and is not desired in most of the cases.
        /// </summary>
        public ImageDescriptor FitUnproportionally()
        {
            AspectRatioElement.Ratio = 0;
            return this;
        }
        
        private ImageDescriptor SetAspectRatio(AspectRatioOption option)
        {
            AspectRatioElement.Ratio = ImageAspectRatio;
            AspectRatioElement.Option = option;
            return this;
        }
        
        #endregion
    }
    
    public static class ImageExtensions
    {
        public static ImageDescriptor Image(this IContainer parent, byte[] imageData)
        {
            var image = Infrastructure.Image.FromBinaryData(imageData);
            return parent.Image(image);
        }
        
        public static ImageDescriptor Image(this IContainer parent, string filePath)
        {
            var image = Infrastructure.Image.FromFile(filePath);
            return parent.Image(image);
        }
        
        public static ImageDescriptor Image(this IContainer parent, Stream fileStream)
        {
            var image = Infrastructure.Image.FromStream(fileStream);
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