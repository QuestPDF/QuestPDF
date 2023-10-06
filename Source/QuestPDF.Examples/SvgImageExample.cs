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
                    container
                        .Padding(25)
                        .Svg(svg);
                });
        }
    }
    
    public static class SvgExtensions
    {
        public static void Svg(this IContainer container, SKSvg svg)
        {
            container
                .AlignCenter()
                .AlignMiddle()
                .ScaleToFit()
                .Width(svg.Picture.CullRect.Width)
                .Height(svg.Picture.CullRect.Height)
                .Canvas((canvas, space) => canvas.DrawPicture(svg.Picture));
        }
    }
}