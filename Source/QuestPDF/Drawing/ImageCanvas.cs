using System;
using System.Collections.Generic;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal sealed class ImageCanvas : SkiaCanvasBase
    {
        private ImageGenerationSettings Settings { get; }
        private SkBitmap Bitmap { get; set; }

        // TODO: consider using SkSurface to cache drawing operations and then encode the surface to an image at the very end of the generation process      
        // this change should reduce memory usage and improve performance
        internal ICollection<byte[]> Images { get; } = new List<byte[]>();
        
        public ImageCanvas(ImageGenerationSettings settings)
        {
            Settings = settings;
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            
        }

        public override void BeginPage(Size size)
        {
            var scalingFactor = Settings.RasterDpi / (float) PageSizes.PointsPerInch;

            Bitmap = new SkBitmap((int) (size.Width * scalingFactor), (int) (size.Height * scalingFactor));
            Canvas = SkCanvas.CreateFromBitmap(Bitmap);
            
            Canvas.Scale(scalingFactor, scalingFactor);
        }

        public override void EndPage()
        {
            Canvas.Save();
            using var imageData = EncodeBitmap();
            var imageBytes = imageData.ToBytes();
            Images.Add(imageBytes);
            
            Bitmap.Dispose();

            SkData EncodeBitmap()
            {
                return Settings.ImageFormat switch
                {
                    ImageFormat.Jpeg => Bitmap.EncodeAsJpeg(Settings.ImageCompressionQuality.ToQualityValue()),
                    ImageFormat.Png => Bitmap.EncodeAsPng(),
                    ImageFormat.Webp => Bitmap.EncodeAsWebp(Settings.ImageCompressionQuality.ToQualityValue()),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}