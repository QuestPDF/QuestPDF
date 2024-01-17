using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class DynamicImageDescriptor
    {
        private Elements.DynamicImage ImageElement { get; }
        
        internal DynamicImageDescriptor(Elements.DynamicImage imageElement)
        {
            ImageElement = imageElement;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.useOriginalImage"]/*' />
        public DynamicImageDescriptor UseOriginalImage(bool value = true)
        {
            ImageElement.UseOriginalImage = value;
            return this;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.rasterDPI"]/*' />
        public DynamicImageDescriptor WithRasterDpi(int dpi)
        {
            ImageElement.TargetDpi = dpi;
            return this;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.compressionQuality"]/*' />
        public DynamicImageDescriptor WithCompressionQuality(ImageCompressionQuality quality)
        {
            ImageElement.CompressionQuality = quality;
            return this;
        }
    }
    
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
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.useOriginalImage"]/*' />
        public ImageDescriptor UseOriginalImage(bool value = true)
        {
            ImageElement.UseOriginalImage = value;
            return this;
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.rasterDPI"]/*' />
        public ImageDescriptor WithRasterDpi(int dpi)
        {
            ImageElement.TargetDpi = dpi;
            return this;
        }

        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.compressionQuality"]/*' />
        public ImageDescriptor WithCompressionQuality(ImageCompressionQuality quality)
        {
            ImageElement.CompressionQuality = quality;
            return this;
        }

        #region Aspect Ratio
        
        /// <summary>
        /// Scales the image to fill the full width of its container. This is the default behavior.
        /// </summary>
        public ImageDescriptor FitWidth()
        {
            return SetAspectRatio(AspectRatioOption.FitWidth);
        }
        
        /// <summary>
        /// <para>The image stretches vertically to fit the full available height.</para>
        /// <para>Often used with height-constraining elements such as: <see cref="ConstrainedExtensions.Height">Height</see>, <see cref="ConstrainedExtensions.MaxHeight">MaxHeight</see>, etc.</para>
        /// </summary>
        public ImageDescriptor FitHeight()
        {
            return SetAspectRatio(AspectRatioOption.FitHeight);
        }
        
        /// <summary>
        /// Combines the FitWidth and FitHeight settings.
        /// The image resizes itself to utilize all available space, preserving its aspect ratio.
        /// It will either fill the width or height based on the container's dimensions.
        /// </summary>
        /// <remarks>
        /// An optimal and safe choice.
        /// </remarks>
        public ImageDescriptor FitArea()
        {
            return SetAspectRatio(AspectRatioOption.FitArea);
        }
        
        /// <summary>
        /// The image adjusts to fill all the available space, disregarding its original proportions.
        /// This can lead to distorted scaling and is generally not recommended for most scenarios.
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
        /// <summary>
        /// Draws an image by decoding it from a provided byte array.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.descriptor"]/*' />
        public static ImageDescriptor Image(this IContainer parent, byte[] imageData)
        {
            var image = Infrastructure.Image.FromBinaryData(imageData);
            return parent.Image(image);
        }
        
        /// <summary>
        /// Draws the image loaded from a file located at the provided path.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.descriptor"]/*' />
        public static ImageDescriptor Image(this IContainer parent, string filePath)
        {
            var image = Infrastructure.Image.FromFile(filePath);
            return parent.Image(image);
        }
        
        /// <summary>
        /// Draws the image loaded from a stream.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.descriptor"]/*' />
        public static ImageDescriptor Image(this IContainer parent, Stream fileStream)
        {
            var image = Infrastructure.Image.FromStream(fileStream);
            return parent.Image(image);
        }
        
        /// <summary>
        /// Draws the <see cref="Infrastructure.Image" /> object. Allows to optimize the generation process.
        /// <a href="https://www.questpdf.com/api-reference/image.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.remarks"]/*' />
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="image.descriptor"]/*' />
        public static ImageDescriptor Image(this IContainer parent, Infrastructure.Image image)
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
        
        /// <summary>
        /// Renders an image of dynamic size dictated by the document layout constraints.
        /// </summary>
        /// <remarks>
        /// Ideal for generating pixel-perfect images that might lose quality upon scaling, such as maps or charts.
        /// </remarks>
        /// <param name="dynamicImageSource">
        /// A delegate that requests an image of desired resolution calculated based on target physical image size and provided DPI.
        /// </param>
        /// <returns>A descriptor for adjusting image attributes like scaling behavior, compression quality, and resolution.</returns>
        public static DynamicImageDescriptor Image(this IContainer element, GenerateDynamicImageDelegate dynamicImageSource)
        {
            var dynamicImage = new DynamicImage
            {
                Source = dynamicImageSource
            };
            
            element.Element(dynamicImage);
            return new DynamicImageDescriptor(dynamicImage);
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