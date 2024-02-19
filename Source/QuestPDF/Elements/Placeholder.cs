using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Placeholder : IComponent
    {
        public string Text { get; set; }

        public void Compose(IContainer container)
        {
            const float imageSvgSize = 24f;
            const string imageSvgPath = "M8.5,13.5L11,16.5L14.5,12L19,18H5M21,19V5C21,3.89 20.1,3 19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19Z";
            
            container
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignMiddle()
                .AlignCenter()
                .Element(x =>
                {
                    if (string.IsNullOrWhiteSpace(Text))
                        x.Height(imageSvgSize).Width(imageSvgSize).SvgPath(imageSvgPath, Colors.White);
                    
                    else
                        x.Text(Text).FontSize(14);
                });
        }
    }
}