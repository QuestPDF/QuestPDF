using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

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
        /// Default value is 72.
        /// </summary>
        public ImageDescriptor WithRasterDpi(int dpi)
        {
            ImageElement.TargetDpi = dpi;
            return this;
        }

        /// <summary>
        /// Encoding quality controls the trade-off between size and quality.
        /// The value 101 corresponds to lossless encoding.
        /// If this value is set to a value between 1 and 100, and the image is opaque, it will be encoded using the JPEG format with that quality setting.
        /// The default value is 90 (very high quality).
        /// </summary>
        public ImageDescriptor WithCompressionQuality(ImageCompressionQuality quality)
        {
            ImageElement.CompressionQuality = quality;
            return this;
        }

        #region Aspect Ratio
        
        public ImageDescriptor FitWidth()
        {
            return SetAspectRatio(AspectRatioOption.FitWidth);
        }
        
        public ImageDescriptor FitHeight()
        {
            return SetAspectRatio(AspectRatioOption.FitHeight);
        }
        
        public ImageDescriptor FitArea()
        {
            return SetAspectRatio(AspectRatioOption.FitArea);
        }
        
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
}