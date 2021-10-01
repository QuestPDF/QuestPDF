using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Placeholder : IComponent
    {
        public string Text { get; set; }
        private static readonly byte[] ImageData;

        static Placeholder()
        {
            ImageData = Helpers.Helpers.LoadEmbeddedResource("QuestPDF.Resources.ImagePlaceholder.png");
        }

        public void Compose(IContainer container)
        {
            container
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignMiddle()
                .AlignCenter()
                .Element(x =>
                {
                    if (string.IsNullOrWhiteSpace(Text))
                        x.MaxHeight(32).Image(ImageData, ImageScaling.FitArea);
                    else
                        x.Text(Text, TextStyle.Default.Size(14));
                });
        }
    }
}