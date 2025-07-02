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
            const string imageSvgPath = "M19,19H5V5H19M19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M13.96,12.29L11.21,15.83L9.25,13.47L6.5,17H17.5L13.96,12.29Z";
            
            container
                .Background(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignMiddle()
                .AlignCenter()
                .Element(x =>
                {
                    if (string.IsNullOrWhiteSpace(Text))
                        x.Height(imageSvgSize).Width(imageSvgSize).SvgPath(imageSvgPath, Colors.Black);
                    
                    else
                        x.Text(Text).FontSize(14);
                });
        }
    }
}