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
            container
                .Background("CCC")
                .AlignMiddle()
                .AlignCenter()
                .MaxHeight(32)
                .Image(ImageData, ImageScaling.FitArea);
        }
    }
}