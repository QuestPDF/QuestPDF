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
                        page.Margin(2, Unit.Centimetre);
                        page.Content().Text("Hello PDF!");
                    });
                })
                .GeneratePdf("hello.pdf");

            Process.Start("explorer.exe", "hello.pdf");
        }
    }
}