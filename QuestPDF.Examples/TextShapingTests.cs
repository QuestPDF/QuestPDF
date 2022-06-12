using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace QuestPDF.Examples
{
    public class TextShapingTests
    {
        [Test]
        public void ShapeText()
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                Typeface = SKTypeface.CreateDefault(),
                IsAntialias = true,
                TextSize = 20
            };
        
            using var backgroundPaint = new SKPaint
            {
                Color = SKColors.LightGray
            };
        
            RenderingTest
                .Create()
                .PageSize(550, 250)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    //var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec odio ipsum, aliquam a neque a, lacinia vehicula lectus.";
                    //var arabic = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا الواجب والعمل سنتنازل غالباً ونرفض الشعور";
        
                    var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
                    var arabic = "ينا الألم. في بعض (5000) الأحيان ونظراً للالتزامات التي يفرضها علينا";
                    
                    container
                        .Padding(25)
                        .Text(arabic)
                        .FontSize(25);
                });
        }
    }
}