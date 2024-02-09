using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.LayoutTests.TestEngine;

internal static class LayoutTestResultVisualization
{
    // output settings
    private const int OutputImageScale = 2;
    private const int Padding = 10;
    
    // document colors
    private static readonly Color DocumentBackgroundColor = Colors.Grey.Darken2;
    private static readonly Color PageBackgroundColor = Colors.Grey.Lighten1;
    private static readonly Color RequiredAreaBackgroundColor = Colors.White;
    
    // grid configuration
    private const float GridSize = 10;
    private const float GridLineThickness = 0.25f;
    private const byte GridLineTransparency = 48;
    
    // mock drawing settings
    private const byte OccludedMockBorderThickness = 5;

    private static readonly Color[] DefaultElementColors =
    {
        Colors.DeepPurple.Lighten2,
        Colors.Blue.Lighten2,
        Colors.Cyan.Lighten2,
        Colors.Green.Lighten2,
        Colors.Lime.Lighten2,
        Colors.Amber.Lighten2,
        Colors.Brown.Lighten2,
        
        Colors.DeepPurple.Medium,
        Colors.Blue.Medium,
        Colors.Cyan.Medium,
        Colors.Green.Medium,
        Colors.Lime.Medium,
        Colors.Amber.Medium,
        Colors.Brown.Medium,
    };
    
    // implementations
    public static void Visualize(LayoutTestResult result, Stream stream)
    {
        // determine output dimenstions
        var numberOfPages = Math.Max(result.ActualLayout.Pages.Count, result.ExpectedLayout.Pages.Count);

        var canvasWidth = result.PageSize.Width * 2 + Padding * 4;
        var canvasHeight = result.PageSize.Height * numberOfPages + Padding * (numberOfPages + 2);

        // create PDF
        using var pdf = SKDocument.CreatePdf(stream);
        using var canvas = pdf.BeginPage(canvasWidth * OutputImageScale, canvasHeight * OutputImageScale);
        
        canvas.Scale(OutputImageScale, OutputImageScale);
        canvas.Clear(new SKColor(DocumentBackgroundColor));

        // draw content
        var mockColors = AssignColorsToMocks();
        DrawDocument();

        // finish generation
        pdf.EndPage();
        pdf.Close();

        IDictionary<string, Color> AssignColorsToMocks()
        {
            var mocks = Enumerable
                .Concat(result.ActualLayout.Pages, result.ExpectedLayout.Pages)
                .SelectMany(x => x.Mocks)
                .Select(x => x.MockId)
                .Distinct()
                .ToList();

            return Enumerable
                .Range(0, mocks.Count)
                .ToDictionary(i => mocks[i], i => DefaultElementColors[i]);
        }

        void DrawDocument()
        {
            canvas.Translate(Padding, Padding);
            
            // draw title
            using var textPaint = new SKPaint
            {
                TextSize = 8,
                Color = SKColors.White,
                Typeface = SKTypeface.FromFamilyName("Calibri", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                TextAlign = SKTextAlign.Center
            };
            
            var actualHeaderPosition = new SKPoint(result.PageSize.Width / 2, textPaint.TextSize / 2);
            canvas.DrawText("ACTUAL", actualHeaderPosition, textPaint);
            
            var expectedHeaderPosition = new SKPoint(Padding * 2 + result.PageSize.Width * 1.5f, textPaint.TextSize / 2);
            canvas.DrawText("EXPECTED", expectedHeaderPosition, textPaint);
            
            // draw pages
            canvas.Save();
            canvas.Translate(0, Padding);
            
            foreach (var pageIndex in Enumerable.Range(0, numberOfPages))
            {
                var actualPage = result.ActualLayout.Pages.ElementAtOrDefault(pageIndex);
                var expectedPage = result.ExpectedLayout.Pages.ElementAtOrDefault(pageIndex);
                
                DrawPage(actualPage);
                DrawLayoutDifferences(actualPage, expectedPage);
                
                canvas.Translate(result.PageSize.Width + Padding, 0);
                canvas.DrawText((pageIndex + 1).ToString(), 0, textPaint.TextSize, textPaint);
                
                canvas.Translate(Padding, 0);
                DrawPage(expectedPage);
                DrawLayoutDifferences(expectedPage, actualPage);
                
                canvas.Translate(-result.PageSize.Width - Padding * 2, result.PageSize.Height + Padding);
            }

            canvas.Restore();
        }
        
        void DrawPage(LayoutTestResult.PageLayout? pageLayout)
        {
            // draw page
            using var availableAreaPaint = new SKPaint
            {
                Color = new SKColor(PageBackgroundColor)
            };
            
            canvas.DrawRect(0, 0, result.PageSize.Width, result.PageSize.Height, availableAreaPaint);
            
            if (pageLayout == null)
            {
                DrawGridLines();
                return;
            }
            
            // draw required area
            using var requiredAreaPaint = new SKPaint
            {
                Color = new SKColor(RequiredAreaBackgroundColor)
            };
            
            canvas.DrawRect(0, 0, pageLayout.RequiredArea.Width, pageLayout.RequiredArea.Height, requiredAreaPaint);
            
            // draw mocks
            foreach (var mock in pageLayout.Mocks)
                DrawMock(mock);
            
            foreach (var mock in pageLayout.Mocks.GetOverlappingItems())
                DrawOccludedMock(mock.Below);
            
            DrawGridLines();
        }

        void DrawMock(LayoutTestResult.MockLayoutPosition mock)
        {
            var color = mockColors[mock.MockId];
                
            using var mockAreaPaint = new SKPaint
            {
                Color = new SKColor(color)
            };
            
            canvas.Save();
            
            canvas.Translate(mock.Position.X, mock.Position.Y);
            canvas.DrawRect(0, 0, mock.Size.Width, mock.Size.Height, mockAreaPaint);

            canvas.Restore();
        }
        
        void DrawOccludedMock(LayoutTestResult.MockLayoutPosition mock)
        {
            var color = mockColors[mock.MockId];
                
            using var mockBorderPaint = new SKPaint
            {
                Color = new SKColor(color),
                IsStroke = true,
                StrokeWidth = OccludedMockBorderThickness
            };

            var borderPosition = new SKRect(0, 0, mock.Size.Width, mock.Size.Height);
            borderPosition.Inflate(-OccludedMockBorderThickness / 2f, -OccludedMockBorderThickness / 2f);
            
            canvas.Save();
            canvas.Translate(mock.Position.X, mock.Position.Y);
            canvas.DrawRect(borderPosition, mockBorderPaint);
            canvas.Restore();
        }
        
        void DrawGridLines()
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Black.WithAlpha(GridLineTransparency),
                StrokeWidth = GridLineThickness
            };

            var verticalLineCount = (int)Math.Floor(result.PageSize.Width / GridSize);
            var horizontalLineCount = (int)Math.Floor(result.PageSize.Height / GridSize);
            
            foreach (var i in Enumerable.Range(1, verticalLineCount))
                canvas.DrawLine(new SKPoint(i * GridSize, 0), new SKPoint(i * GridSize, result.PageSize.Height), paint);
            
            foreach (var i in Enumerable.Range(1, horizontalLineCount))
                canvas.DrawLine(new SKPoint(0, i * GridSize), new SKPoint(result.PageSize.Width, i * GridSize), paint);
        }

        void DrawLayoutDifferences(LayoutTestResult.PageLayout? target, LayoutTestResult.PageLayout? compareWith)
        {
            using var targetPath = BuildPathFromLayout(target);
            using var compareWithPath = BuildPathFromLayout(compareWith);

            using var differencePath = targetPath.Op(compareWithPath, SKPathOp.Difference);
            
            AnnotateInvalidAreaHelper.Annotate(canvas, differencePath);
            
            SKPath BuildPathFromLayout(LayoutTestResult.PageLayout? layout)
            {
                var resultPath = new SKPath();

                if (layout == null)
                    return resultPath;
                
                foreach (var mock in layout.Mocks)
                {
                    var position = new SKRect(
                        mock.Position.X, 
                        mock.Position.Y, 
                        mock.Position.X + mock.Size.Width, 
                        mock.Position.Y + mock.Size.Height);
                    
                    resultPath.AddRect(position);
                }

                return resultPath;
            }
        }
    }
}