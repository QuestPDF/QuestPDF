using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal sealed class DebugArea : IComponent
    {
        public IElement? Child { get; set; }
        
        public string Text { get; set; }
        public string Color { get; set; } = Colors.Red.Medium;
        public void Compose(IContainer container)
        {
            var backgroundColor = SKColor.Parse(Color).WithAlpha(50).ToString();
            
            container
                .Border(1)
                .BorderColor(Color)
                .Layers(layers =>
                {
                    layers.PrimaryLayer().Element(Child);
                    layers.Layer().Background(backgroundColor);
                    
                    layers
                        .Layer()
                        .ShowIf(!string.IsNullOrWhiteSpace(Text))
                        .AlignCenter()
                        .MinimalBox()
                        .Background(Colors.White)
                        .Padding(2)
                        .Text(Text)
                        .FontColor(Color)
                        .FontFamily(Fonts.Consolas)
                        .FontSize(8);
                });
        }
    }
}