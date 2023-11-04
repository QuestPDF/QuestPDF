using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal class LayoutTestResultVisualization
{
    private const string DocumentBackgroundColor = Colors.Grey.Lighten1;
    private const string PageBackgroundColor = Colors.Grey.Lighten3;
    private const string RequiredAreaBackgroundColor = Colors.White;
    private const string GridLineColor = Colors.Grey.Lighten1;

    private static readonly string[] DefaultElementColors =
    {
        Colors.Red.Medium,
        Colors.Green.Medium,
        Colors.Blue.Medium,
        Colors.Pink.Medium,
        Colors.Orange.Medium,
        Colors.Lime.Medium,
        Colors.Cyan.Medium,
        Colors.Indigo.Medium
    };
    
    public static void Visualize(LayoutTestResult result)
    {
        var path = "test.pdf";
        
        if (File.Exists(path))
            File.Delete(path);
        
        // determine children colors
        var mocks = Enumerable
            .Concat(result.GeneratedLayout, result.ExpectedLayout)
            .SelectMany(x => x.MockPositions)
            .Select(x => x.MockId)
            .Distinct()
            .ToList();

        var colors = Enumerable
            .Range(0, mocks.Count)
            .ToDictionary(i => mocks[i], i => DefaultElementColors[i]);

        // create new pdf document output
        var matrixHeight = Math.Max(result.GeneratedLayout.Count, result.ExpectedLayout.Count);
        
        const int pagePadding = 25;
        var imageInfo = new SKImageInfo((int)result.PageSize.Width * 2 + pagePadding * 4, (int)(result.PageSize.Height * matrixHeight + pagePadding * (matrixHeight + 2)));

        const int outputScale = 2;
        
        using var pdf = SKDocument.CreatePdf(path);
        using var canvas = pdf.BeginPage(imageInfo.Width * outputScale, imageInfo.Height * outputScale);
        
        canvas.Scale(outputScale, outputScale);
        
        // page background
        canvas.Clear(SKColor.Parse(DocumentBackgroundColor));
        
        // chain titles
        
        // available area
        using var titlePaint = TextStyle.LibraryDefault.FontSize(16).Bold().ToPaint().Clone();
        titlePaint.TextAlign = SKTextAlign.Center;

        canvas.Save();
        
        canvas.Translate(pagePadding + result.PageSize.Width / 2f, pagePadding + titlePaint.TextSize / 2);
        canvas.DrawText("RESULT", 0, 0, titlePaint);
        
        canvas.Translate(pagePadding * 2 + result.PageSize.Width, 0);
        canvas.DrawText("EXPECTED", 0, 0, titlePaint);
        
        canvas.Restore();

        // side visualization
        canvas.Save();
        
        canvas.Translate(pagePadding, pagePadding * 2);
        DrawSide(result.GeneratedLayout);
        
        canvas.Translate(result.PageSize.Width + pagePadding * 2, 0);
        DrawSide(result.ExpectedLayout);
        
        canvas.Restore();
        
        
        // draw page numbers
        canvas.Save();
        
        canvas.Translate(pagePadding * 2 + result.PageSize.Width, pagePadding * 2 + titlePaint.TextSize);
        
        foreach (var i in Enumerable.Range(0, matrixHeight))
        {
            canvas.DrawText((i + 1).ToString(), 0, 0, titlePaint);
            canvas.Translate(0, pagePadding + result.PageSize.Height);
        }
        
        canvas.Restore();

        pdf.EndPage();
        pdf.Close();
        
        void DrawSide(ICollection<LayoutTestResult.PageLayoutSnapshot> commands)
        {
            canvas.Save();
            
            foreach (var pageDrawingCommand in commands)
            {
                DrawPage(pageDrawingCommand);
                canvas.Translate(0, result.PageSize.Height + pagePadding);
            }
            
            canvas.Restore();
        }

        void DrawPageGrid(Size pageSize)
        {
            using var paint = new SKPaint
            {
                Color = SKColor.Parse(GridLineColor),
                StrokeWidth = 1
            };

            const float GridSize = 10f;
            
            foreach (var i in Enumerable.Range(1, (int)Math.Floor(pageSize.Width / GridSize)))
            {
                canvas.DrawLine(new SKPoint(i * GridSize, 0), new SKPoint(i * GridSize, pageSize.Height), paint);
            }
            
            foreach (var i in Enumerable.Range(1, (int)Math.Floor(pageSize.Height / GridSize)))
            {
                canvas.DrawLine(new SKPoint(0, i * GridSize), new SKPoint(pageSize.Width, i * GridSize), paint);
            }
        }

        void DrawPage(LayoutTestResult.PageLayoutSnapshot command)
        {
            // available area
            using var availableAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(PageBackgroundColor)
            };
            
            canvas.DrawRect(0, 0, result.PageSize.Width, result.PageSize.Height, availableAreaPaint);
            
            // taken area
            using var takenAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(RequiredAreaBackgroundColor)
            };
            
            canvas.DrawRect(0, 0, command.RequiredArea.Width, command.RequiredArea.Height, takenAreaPaint);
        
            DrawPageGrid(result.PageSize);
            
            // draw children
            foreach (var child in command.MockPositions)
            {
                canvas.Save();

                const float strokeWidth = 3f;

                var color = colors[child.MockId];
            
                using var childBorderPaint = new SKPaint
                {
                    Color = SKColor.Parse(color),
                    IsStroke = true,
                    StrokeWidth = strokeWidth
                };
            
                using var childAreaPaint = new SKPaint
                {
                    Color = SKColor.Parse(color).WithAlpha(128)
                };
            
                canvas.Translate(child.Position.X, child.Position.Y);
                canvas.DrawRect(0, 0, child.Size.Width, child.Size.Height, childAreaPaint);
                canvas.DrawRect(strokeWidth / 2, strokeWidth / 2, child.Size.Width - strokeWidth, child.Size.Height - strokeWidth, childBorderPaint);
            
                canvas.Restore();
            }
        }
        
        // save
        GenerateExtensions.OpenFileUsingDefaultProgram(path);
    }
}