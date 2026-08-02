using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.DynamicComposition;

public class DynamicCompositionNestedContentExample
{
    [Test]
    public void Example()
    {
        var catalog = new CategoryNode("All products", 231, [
            new("Electronics", 154, [
                new("Computers", 89, [
                    new("Laptops", 52, []),
                    new("Desktops", 37, [])
                ]),
                new("Audio", 65, [])
            ]),
            new("Office supplies", 77, [
                new("Paper", 41, []),
                new("Writing instruments", 36, [])
            ])
        ]);

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(450, 0));
                    page.MaxSize(new PageSize(450, 1000));
                    page.DefaultTextStyle(x => x.FontSize(18));
                    page.Margin(25);

                    page.Content().Element(container => ComposeCategory(container, catalog));
                });
            })
            .GenerateImages(x => "dynamic-composition-recursive-content.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }

    private void ComposeCategory(IContainer container, CategoryNode node, int depth = 0)
    {
        container.Column(column =>
        {
            column.Item()
                .PaddingLeft(depth * 25)
                .Text(text =>
                {
                    text.Span(node.Name).SemiBold();
                    text.Span($"  ({node.ProductCount} products)").FontColor(Colors.Grey.Medium);
                });

            foreach (var child in node.Children)
                column.Item().PaddingTop(8).Element(childContainer => ComposeCategory(childContainer, child, depth + 1));
        });
    }
}

public sealed record CategoryNode(string Name, int ProductCount, List<CategoryNode> Children);
