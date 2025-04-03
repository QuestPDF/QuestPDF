using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing.DocumentCanvases
{
    internal sealed class ImageDocumentCanvas : IDocumentCanvas, IDisposable
    {
        private ImageGenerationSettings Settings { get; }
        private SkBitmap Bitmap { get; set; }
        
        private SkDocument Document { get; }
        private SkCanvas? CurrentPageCanvas { get; set; }
        private ProxyDrawingCanvas DrawingCanvas { get; } = new();
        
        internal ICollection<byte[]> Images { get; } = new List<byte[]>();
        
        public ImageDocumentCanvas(ImageGenerationSettings settings)
        {
            Settings = settings;
        }

        #region IDisposable
        
        ~ImageDocumentCanvas()
        {
            this.WarnThatFinalizerIsReached();
            Dispose();
        }
        
        public void Dispose()
        {
            CurrentPageCanvas?.Dispose();
            Bitmap?.Dispose();
            GC.SuppressFinalize(this);
        }
        
        #endregion
        
        #region IDocumentCanvas
        
        public void BeginDocument()
        {
            
        }

        public void EndDocument()
        {
            CurrentPageCanvas?.Dispose();
            Bitmap?.Dispose();
        }

        public void BeginPage(Size size)
        {
            var scalingFactor = Settings.RasterDpi / (float) PageSizes.PointsPerInch;

            Bitmap = new SkBitmap((int) (size.Width * scalingFactor), (int) (size.Height * scalingFactor));
            CurrentPageCanvas = SkCanvas.CreateFromBitmap(Bitmap);
            
            CurrentPageCanvas.Scale(scalingFactor, scalingFactor);
            
            DrawingCanvas.Target = new SkiaDrawingCanvas(size.Width, size.Height);
            DrawingCanvas.SetZIndex(0);
        }

        public void EndPage()
        {
            Debug.Assert(CurrentPageCanvas != null);
            
            using var documentPageSnapshot = DrawingCanvas.GetSnapshot();
            documentPageSnapshot.DrawOnSkCanvas(CurrentPageCanvas);
            
            CurrentPageCanvas.Save();
            CurrentPageCanvas.Dispose();
            
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
        
        public IDrawingCanvas GetDrawingCanvas()
        {
            return DrawingCanvas;
        }
        
        #endregion
    }
}