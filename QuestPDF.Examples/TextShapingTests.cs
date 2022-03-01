using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
                    var arabic = "ينا الألم. في بعض الأحيان ونظراً للالتزامات التي يفرضها علينا";
                    
                    var text = arabic;
                    var metrics = textPaint.FontMetrics;
                    
                    container
                        .Padding(25)
                        .Canvas((canvas, space) =>
                        {
                            canvas.Translate(0, 20);
                            
                            var width = MeasureText(text, textPaint);
                            var widthReal = textPaint.MeasureText(text);
                            canvas.DrawRect(0, metrics.Descent, width, metrics.Ascent - metrics.Descent, backgroundPaint);
                            
                            canvas.DrawShapedText(text, 0, 0, textPaint);
                            
                            canvas.Translate(0, 40);
                            canvas.DrawText(text, 0, 0, textPaint);
                        });
                });
        }
        
        float MeasureText(string text, SKPaint paint)
        {
            var font = paint.ToFont();
            using var shaper = new SKShaper(paint.Typeface);
            var result = shaper.Shape(text + " ", paint);
            return Math.Max(result.Points.First().X, result.Points.Last().X);

            var glyphCount = result.Codepoints.Length;
            var glyphIds = new ushort[glyphCount];
            var glyphWidths = new float[glyphCount];
            var glyphBounds = new SKRect[glyphCount];
            
            font.GetGlyphs(result.Codepoints.Select(x => (int)x).ToArray(), glyphIds);
            font.GetGlyphWidths(glyphIds, glyphWidths, glyphBounds);

            return glyphWidths.Sum();
            
            return result.Points.Max(p => p.X);

            using var skTextBlobBuilder = new SKTextBlobBuilder();

            var positionedRunBuffer = skTextBlobBuilder.AllocatePositionedRun(font, result.Codepoints.Length);
            var glyphSpan = positionedRunBuffer.GetGlyphSpan();
            var positionSpan = positionedRunBuffer.GetPositionSpan();
            
            for (int index = 0; index < result.Codepoints.Length; ++index)
            {
                glyphSpan[index] = (ushort) result.Codepoints[index];
                positionSpan[index] = result.Points[index];
            }

            using var text1 = skTextBlobBuilder.Build();
            return text1.Bounds.Width;
        }
        
        [Test]
        public void ShapeSingle()
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                Typeface = SKTypeface.CreateDefault(),
                IsAntialias = true,
                TextSize = 20
            };

            var text = string.Join(" ", Enumerable.Range(0, 10_000).Select(x => Placeholders.Sentence()));
            
            using var shaper = new SKShaper(textPaint.Typeface);
            var result = shaper.Shape(text + " ", textPaint);
        }
        
        [Test]
        public void ShapeMany()
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                Typeface = SKTypeface.CreateDefault(),
                IsAntialias = true,
                TextSize = 20
            };

            var text = string.Join(" ", Enumerable.Range(0, 1_000).Select(x => Placeholders.Sentence()));
            
            foreach (var part in Regex.Split(text, "( )|(\\w+)"))
            {
                using var shaper = new SKShaper(textPaint.Typeface);
                var result = shaper.Shape(part + " ", textPaint);
            }
        }
        
        [Test]
        public void ShapeMany2()
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                Typeface = SKTypeface.CreateDefault(),
                IsAntialias = true,
                TextSize = 20
            };

            using var shaper = new SKShaper(textPaint.Typeface);
            
            foreach (var i in Enumerable.Range(0, 10_000))
            {
                var text = Placeholders.Paragraph();

                textPaint.MeasureText(text);
                var result = shaper.Shape(text + " ", textPaint);
            }
        }
    }
}