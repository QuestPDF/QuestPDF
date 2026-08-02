using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.DynamicComposition;

public class DynamicCompositionMixedContentBlocksExample
{
    [Test]
    public void Example()
    {
        var blocks = new List<ContentBlock>
        {
            new HeadingBlock("Quarterly Product Update"),
            new ParagraphBlock(Placeholders.Paragraph()),
            new ImageBlock(Placeholders.Image(600, 200)),
            new QuoteBlock("This release cut our document generation time in half.", "Anna Kowalska, Operations Lead"),
            new ParagraphBlock(Placeholders.Paragraph())
        };

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(500, 0));
                    page.MaxSize(new PageSize(500, 1000));
                    page.DefaultTextStyle(x => x.FontSize(16));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            foreach (var block in blocks)
                                column.Item().Element(container => ComposeBlock(container, block));
                        });
                });
            })
            .GenerateImages(x => "dynamic-composition-content-blocks.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }

    private void ComposeBlock(IContainer container, ContentBlock block)
    {
        if (block is HeadingBlock heading)
        {
            container.Text(heading.Text).FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
            return;
        }

        if (block is ParagraphBlock paragraph)
        {
            container.Text(paragraph.Text);
            return;
        }

        if (block is ImageBlock image)
        {
            container.Image(image.Data);
            return;
        }

        if (block is QuoteBlock quote)
        {
            container
                .BorderLeft(3)
                .BorderColor(Colors.Blue.Medium)
                .PaddingLeft(15)
                .Column(column =>
                {
                    column.Item().Text(quote.Text).Italic();
                    column.Item().PaddingTop(5).Text($"— {quote.Author}").FontColor(Colors.Grey.Darken1);
                });

            return;
        }

        throw new NotSupportedException($"Unsupported content block: {block.GetType().Name}");
    }
}

public abstract record ContentBlock;
public sealed record HeadingBlock(string Text) : ContentBlock;
public sealed record ParagraphBlock(string Text) : ContentBlock;
public sealed record ImageBlock(byte[] Data) : ContentBlock;
public sealed record QuoteBlock(string Text, string Author) : ContentBlock;
