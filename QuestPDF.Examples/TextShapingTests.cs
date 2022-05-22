using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace QuestPDF.Examples
{
    public class TextShapingTests
    {
        // [Test]
        // public void ShapeText()
        // {
        //     using var textPaint = new SKPaint
        //     {
        //         Color = SKColors.Black,
        //         Typeface = SKTypeface.CreateDefault(),
        //         IsAntialias = true,
        //         TextSize = 20
        //     };
        //
        //     using var backgroundPaint = new SKPaint
        //     {
        //         Color = SKColors.LightGray
        //     };
        //
        //     RenderingTest
        //         .Create()
        //         .PageSize(550, 250)
        //         .ProduceImages()
        //         .ShowResults()
        //         .Render(container =>
        //         {
        //             //var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec odio ipsum, aliquam a neque a, lacinia vehicula lectus.";
        //             //var arabic = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا الواجب والعمل سنتنازل غالباً ونرفض الشعور";
        //
        //             var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        //             var arabic = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا";
        //
        //             var text = arabic;
        //             var metrics = textPaint.FontMetrics;
        //
        //             container
        //                 .Padding(25)
        //                 .Canvas((canvas, space) =>
        //                 {
        //                     canvas.Translate(0, 20);
        //
        //                     var width = MeasureText(text, textPaint);
        //                     var widthReal = textPaint.MeasureText(text);
        //                     canvas.DrawRect(0, metrics.Descent, width, metrics.Ascent - metrics.Descent, backgroundPaint);
        //
        //                     canvas.DrawShapedText(text, 0, 0, textPaint);
        //
        //                     canvas.Translate(0, 40);
        //                     canvas.DrawText(text, 0, 0, textPaint);
        //                 });
        //         });
        // }

        [Test]
        public void MeasureTest()
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                Typeface = SKTypeface.CreateDefault(),
                IsAntialias = true,
                TextSize = 20
            };
            
            var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec odio ipsum, aliquam a neque a, lacinia vehicula lectus.";
            var arabic = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا";
            //            012345678901234567890123456789012345678901234567890123456
            var shaper = new SKShaper(textPaint.Typeface);
            var result = shaper.Shape(lorem, textPaint);
        } 
    }
}