using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element
    {
        public byte[]? Data { get; set; }
        private SKImage? InternalImage { get; set; }

        ~Image()
        {
            InternalImage?.Dispose();
        }

        private void Initialize()
        {
            if (Data != null)
                InternalImage ??= SKImage.FromEncodedData(Data);
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            Initialize();
            
            if (InternalImage == null)
                return new FullRender(Size.Zero);
            
            if (availableSpace.Width < 0 || availableSpace.Height < 0)
                return new Wrap();
            
            var size = GetTargetSize(availableSpace);
            return new FullRender(size);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Initialize();
            
            if (InternalImage == null)
                return;

            var size = GetTargetSize(availableSpace);
            canvas.DrawImage(InternalImage, Position.Zero, size);
        }

        private Size GetTargetSize(Size availableSpace)
        {
            var imageRatio = InternalImage.Width / (float)InternalImage.Height;
            var spaceRatio = availableSpace.Width / (float) availableSpace.Height;

            return imageRatio < spaceRatio 
                ? new Size((int)(availableSpace.Height * imageRatio), availableSpace.Height) 
                : new Size(availableSpace.Width, (int)(availableSpace.Width / imageRatio));
        }
    }
}