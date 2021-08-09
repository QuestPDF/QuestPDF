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
                .PageSize(600, 400)
                .FileName()
                .ProducePdf()
                .Render(container =>
                {
                    container.Padding(20).Text(text =>
                    {
                        text.Span("Let's start with something bold...", TextStyle.Default.SemiBold().Size(18));
                        text.Span("And BIG...", TextStyle.Default.Size(28).Color(Colors.DeepOrange.Darken2).BackgroundColor(Colors.Yellow.Lighten3).Underlined());
                        text.Span(Placeholders.LoremIpsum(), TextStyle.Default.Size(16));
                        //text.Element().ExternalLink("https://www.questpdf.com/").Width(200).Height(50).Text("Visit questpdf.com", TextStyle.Default.Underlined().Color(Colors.Blue.Darken2));
                        text.Span(Placeholders.LoremIpsum(), TextStyle.Default.Size(16).Stroked());
                        text.Span("And now it's time for some colors 12345 678 90293 03490 83290.", TextStyle.Default.Size(20).Color(Colors.Green.Medium));
                    });
                });
        }
    }
}