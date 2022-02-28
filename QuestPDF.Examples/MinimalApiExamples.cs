using System.Diagnostics;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ShimSkiaSharp;
using SKTypeface = SkiaSharp.SKTypeface;

namespace QuestPDF.Examples
{
    public class MinimalApiExamples
    {
        [Test]
        public void MinimalApi()
        {
            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A5);
                        page.Margin(1.5f, Unit.Centimetre);
                        
                        page.Header()
                            .Text("Hello PDF!", TextStyle.Default.SemiBold().Size(20));
                        
                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Spacing(10);
                                
                                x.Item().Text(Placeholders.LoremIpsum());
                                x.Item().Image(Placeholders.Image(200, 100));
                            });
                    });
                })
                .GeneratePdf("hello.pdf");

            Process.Start("explorer.exe", "hello.pdf");
        }
    }
}