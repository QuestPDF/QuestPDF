using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternConfigurableComponentExample
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(600, 1200));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Component(BuildSampleSection());
                });
            })
            .GenerateImages(x => $"code-pattern-component-configurable.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });

        IComponent BuildSampleSection()
        {
            var section = new SectionComponent();

            section.Text("Product name", Placeholders.Label());
            section.Text("Description", Placeholders.Sentence());
            section.Text("Price", Placeholders.Price());
            section.Text("Date of production", Placeholders.ShortDate());
            section.Image("Photo of the product", "Resources/product.jpg");
            section.Custom("Status").Text("Accepted").FontColor(Colors.Green.Darken2).Bold();
            
            return section;
        }
    }

    public class SectionComponent : IComponent
    {
        private List<(string Label, IContainer Content)> Fields { get; set; } = [];

        public SectionComponent()
        {
            
        }
        
        public void Compose(IContainer container)
        {
            container
                .Border(1)
                .Column(column =>
                {
                    foreach (var field in Fields)
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Border(1)
                                .BorderColor(Colors.Grey.Medium)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Text(field.Label);

                            row.RelativeItem(2)
                                .Border(1)
                                .BorderColor(Colors.Grey.Medium)
                                .Padding(10)
                                .Element(field.Content);
                        });
                    }
                });
        }

        public void Text(string label, string text)
        {
            Custom(label).Text(text);
        }
        
        public void Image(string label, string imagePath)
        {
            Custom(label).Image(imagePath);
        }
        
        public IContainer Custom(string label)
        {
            var content = EmptyContainer.Create();
            Fields.Add((label, content));
            return content;
        }
    }
}