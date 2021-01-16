using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Watermark : Element
    {
        private Position Offset { get; set; } = new Position(36, 36);
        private const float ImageHeight = 28;
        private const string TargetUrl = "https://www.questpdf.com";
        
        private Image Image { get; set; }
        private static readonly byte[] ImageData;

        static Watermark()
        {
            ImageData = Helpers.Helpers.LoadEmbeddedResource("QuestPDF.Resources.Watermark.png");
        }
        
        public Watermark()
        {
            Image = new Image()
            {
                Data = ImageData
            };
        }

        internal void AdjustPosition(Element? element)
        {
            while (element != null)
            {
                if (element is Padding padding)
                {
                    if (padding.Left > 0 && padding.Bottom > 0)
                        Offset = new Position(padding.Left, padding.Bottom);
                    
                    return;
                }

                element = (element as ContainerElement)?.Child;
            }
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Image.Measure(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var offset = new Position(Offset.X, availableSpace.Height - Offset.Y - ImageHeight);
            canvas.Translate(offset);

            availableSpace = new Size(availableSpace.Width, ImageHeight);
            var targetSize = Image.Measure(availableSpace) as FullRender;
            Image.Draw(canvas, targetSize);
            canvas.DrawLink(TargetUrl, targetSize);
            
            canvas.Translate(offset.Reverse());
        }
    }
}