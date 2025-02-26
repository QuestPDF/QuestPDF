using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples.CodePatterns;

public class CodePatternLocalHelpersExample
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
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Text("Business details:").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            
                            AddContactItem("Resources/Icons/phone.svg", Placeholders.PhoneNumber());
                            AddContactItem("Resources/Icons/email.svg", Placeholders.Email());
                            AddContactItem("Resources/Icons/web.svg", Placeholders.WebpageUrl());

                            void AddContactItem(string iconPath, string label)
                            {
                                column.Item().Row(row =>
                                {
                                    row.ConstantItem(32).AspectRatio(1).Svg(iconPath);
                                    row.ConstantItem(15);
                                    row.AutoItem().AlignMiddle().Text(label);
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => $"code-pattern-local-helpers.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}