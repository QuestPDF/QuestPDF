using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TextExamples
    {
        [Test]
        public void TextElements()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .FileName()
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span(Placeholders.LoremIpsum());

                            text.NewLine();

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ", TextStyle.Default.Bold());
                            text.Span("this is a red and underlined text, ", TextStyle.Default.Color(Colors.Red.Medium).Underlined());
                            text.Span("and this is slightly bigger text.", TextStyle.Default.Size(16));

                            text.NewLine();

                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-10).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");

                            text.NewLine();

                            text.Span("This is page number ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();

                            text.NewLine();

                            text.ExternalLocation("Please visit QuestPDF website", "https://www.questpdf.com");
                            
                            text.NewLine();
                       
                            text.Span(Placeholders.Sentence());
                        });
                });
        }

        [Test]
        public void SpaceIssue()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .FileName()
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(5)
                        .Padding(10)
                        .Debug()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span(Placeholders.LoremIpsum());

                            text.NewLine();

                            text.Span("This text is a normal text, ");
                            text.Span("this is a bold text, ", TextStyle.Default.Bold());
                            text.Span("this is a red and underlined text, ", TextStyle.Default.Color(Colors.Red.Medium).Underlined());
                            text.Span("and this is slightly bigger text.", TextStyle.Default.Size(16));

                            text.NewLine();

                            text.Span("The new text element also supports injecting custom content between words: ");
                            text.Element().PaddingBottom(-10).Height(16).Width(32).Image(Placeholders.Image);
                            text.Span(".");

                            text.NewLine();

                            text.Span("This is page number ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();

                            text.NewLine();

                            text.ExternalLocation("Please visit QuestPDF website", "https://www.questpdf.com");
                            
                            text.NewLine();
                       
                            text.Span("This is target text that does not show up.");
                        });
                });
        }
        
        [Test]
        public void PageNumber()
        {
            RenderingTest
                .Create()
                .PageSize(500, 400)
                .FileName()
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Box()
                        .Border(1)
                        .Padding(10)
                        .Text(text =>
                        {
                            text.DefaultTextStyle(TextStyle.Default);
                            text.AlignLeft();
                            text.ParagraphSpacing(10);

                            text.Span("This is page number ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();
                        });
                });
        }
    }
}