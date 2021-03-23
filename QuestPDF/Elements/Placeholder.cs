using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Placeholder : IComponent
    {
        private static readonly byte[] ImageData;

        static Placeholder()
        {
            ImageData = Helpers.Helpers.LoadEmbeddedResource("QuestPDF.Resources.ImagePlaceholder.png");
        }

        public void Compose(IContainer container)
        {
            // TODO: consider moving this element into fluent API
            
            container
                .Background("CCC")
                .AlignMiddle()
                .AlignCenter()
                .MaxHeight(32)
                .Image(ImageData, ImageScaling.FitArea);
        }
    }
}