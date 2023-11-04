using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal class LayoutTestResultVisualization
{
    // visual settings
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

    private const int Padding = 32;
    private const int OutputImageScale = 2;

    private const float GridSize = 10;
    
    private const float MockBorderThickness = 3;
    private const byte MockBackgroundOpacity = 128;
    
    // implementations
    public static void Visualize(LayoutTestResult result, Stream stream)
    {
        // determine output dimenstions
        var numberOfPages = Math.Max(result.ActualLayout.Count, result.ExpectedLayout.Count);

        var canvasWidth = result.PageSize.Width * 2 + Padding * 4;
        var canvasHeight = result.PageSize.Height * numberOfPages + Padding * (numberOfPages + 2);

        // create PDF
        using var pdf = SKDocument.CreatePdf(stream);
        using var canvas = pdf.BeginPage(canvasWidth * OutputImageScale, canvasHeight * OutputImageScale);
        
        canvas.Scale(OutputImageScale, OutputImageScale);
        canvas.Clear(SKColor.Parse(DocumentBackgroundColor));

        // draw content
        var mockColors = AssignColorsToMocks();
        
        canvas.Translate(Padding, Padding);
        DrawLayout("ACTUAL", result.ActualLayout);
        
        canvas.Translate(result.PageSize.Width + Padding, 0);
        DrawPageNumbers();
        
        canvas.Translate(Padding, 0);
        DrawLayout("EXPECTED", result.ActualLayout);

        // finish generation
        pdf.EndPage();
        pdf.Close();

        IDictionary<string, string> AssignColorsToMocks()
        {
            var mocks = Enumerable
                .Concat(result.ActualLayout, result.ExpectedLayout)
                .SelectMany(x => x.MockPositions)
                .Select(x => x.MockId)
                .Distinct()
                .ToList();

            return Enumerable
                .Range(0, mocks.Count)
                .ToDictionary(i => mocks[i], i => DefaultElementColors[i]);
        }
        
        void DrawLayout(string title, ICollection<LayoutTestResult.PageLayoutSnapshot> commands)
        {
            // draw title
            using var titlePaint = TextStyle.LibraryDefault.FontSize(16).Bold().ToPaint().Clone();
            titlePaint.TextAlign = SKTextAlign.Center;

            var titlePosition = new SKPoint(result.PageSize.Width / 2, titlePaint.TextSize / 2);
            canvas.DrawText(title, titlePosition, titlePaint);
            
            // draw pages
            canvas.Save();
            canvas.Translate(0, Padding);
            
            foreach (var pageDrawingCommand in commands)
            {
                DrawPage(pageDrawingCommand);
                canvas.Translate(0, result.PageSize.Height + Padding);
            }
            
            canvas.Restore();
        }

        void DrawPageNumbers()
        {
            using var textPaint = TextStyle.LibraryDefault.FontSize(16).Bold().ToPaint().Clone();
            textPaint.TextAlign = SKTextAlign.Center;
            
            canvas.Save();
        
            canvas.Translate(0, Padding + textPaint.TextSize);
        
            foreach (var pageNumber in Enumerable.Range(1, numberOfPages))
            {
                canvas.DrawText(pageNumber.ToString(), 0, 0, textPaint);
                canvas.Translate(0, Padding + result.PageSize.Height);
            }
        
            canvas.Restore();
        }

        void DrawPage(LayoutTestResult.PageLayoutSnapshot pageLayout)
        {
            // draw page
            using var availableAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(PageBackgroundColor)
            };
            
            using var requiredAreaPaint = new SKPaint
            {
                Color = SKColor.Parse(RequiredAreaBackgroundColor)
            };
            
            canvas.DrawRect(0, 0, result.PageSize.Width, result.PageSize.Height, availableAreaPaint);
            canvas.DrawRect(0, 0, pageLayout.RequiredArea.Width, pageLayout.RequiredArea.Height, requiredAreaPaint);
            
            DrawGridLines();
            
            // draw mocks
            foreach (var mock in pageLayout.MockPositions)
            {
                canvas.Save();

                var color = mockColors[mock.MockId];
            
                using var mockBorderPaint = new SKPaint
                {
                    Color = SKColor.Parse(color),
                    IsStroke = true,
                    StrokeWidth = MockBorderThickness
                };
            
                using var mockAreaPaint = new SKPaint
                {
                    Color = SKColor.Parse(color).WithAlpha(MockBackgroundOpacity)
                };
            
                canvas.Translate(mock.Position.X, mock.Position.Y);
                canvas.DrawRect(0, 0, mock.Size.Width, mock.Size.Height, mockAreaPaint);
                canvas.DrawRect(MockBorderThickness / 2, MockBorderThickness / 2, mock.Size.Width - MockBorderThickness, mock.Size.Height - MockBorderThickness, mockBorderPaint);
            
                canvas.Restore();
            }
        }
        
        void DrawGridLines()
        {
            using var paint = new SKPaint
            {
                Color = SKColor.Parse(GridLineColor),
                StrokeWidth = 1
            };

            var verticalLineCount = (int)Math.Floor(result.PageSize.Width / GridSize);
            var horizontalLineCount = (int)Math.Floor(result.PageSize.Height / GridSize);
            
            foreach (var i in Enumerable.Range(1, verticalLineCount))
                canvas.DrawLine(new SKPoint(i * GridSize, 0), new SKPoint(i * GridSize, result.PageSize.Height), paint);
            
            foreach (var i in Enumerable.Range(1, horizontalLineCount))
                canvas.DrawLine(new SKPoint(0, i * GridSize), new SKPoint(result.PageSize.Width, i * GridSize), paint);
        }
    }
}