using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Svg.Skia;

namespace QuestPDF.Examples
{
    public class SvgImageExample
    {
        [Test]
        public void ImageSVG()
        {
            using var svg = new SKSvg();
            svg.Load("pdf-icon.svg");
            
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container.Svg(SvgImage.FromFile("pdf-icon.svg")).FitArea();
                });
        }
    }
}