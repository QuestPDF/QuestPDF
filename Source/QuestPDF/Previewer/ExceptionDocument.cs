using System;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Previewer
{
    public class ExceptionDocument : IDocument
    {
        private Exception Exception { get; }
    
        public ExceptionDocument(Exception exception)
        {
            Exception = exception;
        }
    
        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }
        
        public DocumentSettings GetSettings()
        {
            return DocumentSettings.Default;
        }

        public void Compose(IDocumentContainer document)
        {
            document.Page(page =>
            {
                page.ContinuousSize(PageSizes.A4.Width);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(16));

                page.Foreground().PaddingTop(5).Border(10).BorderColor(Colors.Red.Medium);
                
                page.Header()
                    .ShowOnce()
                    .PaddingBottom(5)
                    .Row(row =>
                    {
                        row.Spacing(15);
                        
                        row.AutoItem()
                            .PaddingTop(15)
                            .Width(48)
                            .AspectRatio(1)
                            .Canvas((canvas, size) =>
                            {
                                const float iconSize = 24;
                                using var iconPath = SKPath.ParseSvgPathData("M23,12L20.56,14.78L20.9,18.46L17.29,19.28L15.4,22.46L12,21L8.6,22.47L6.71,19.29L3.1,18.47L3.44,14.78L1,12L3.44,9.21L3.1,5.53L6.71,4.72L8.6,1.54L12,3L15.4,1.54L17.29,4.72L20.9,5.54L20.56,9.22L23,12M20.33,12L18.5,9.89L18.74,7.1L16,6.5L14.58,4.07L12,5.18L9.42,4.07L8,6.5L5.26,7.09L5.5,9.88L3.67,12L5.5,14.1L5.26,16.9L8,17.5L9.42,19.93L12,18.81L14.58,19.92L16,17.5L18.74,16.89L18.5,14.1L20.33,12M11,15H13V17H11V15M11,7H13V13H11V7");
                                
                                using var paint = new SKPaint()
                                { 
                                    Color = SKColors.Red,
                                    IsAntialias = true
                                };
                                
                                canvas.Scale(size.Width / iconSize);
                                canvas.DrawPath(iconPath, paint);
                            });
                        
                        row.RelativeItem()
                            .Column(column =>
                            {
                                column.Item().Text("Exception").FontSize(36).FontColor(Colors.Red.Medium).Bold();
                                column.Item().PaddingTop(-10).Text("Don't panic! Just analyze what's happened...").FontSize(18).FontColor(Colors.Red.Medium).Bold();
                            }); 
                    });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    var currentException = Exception;
                    
                    while (currentException != null)
                    {
                        column.Item()
                            .PaddingTop(25)
                            .PaddingBottom(15)
                            
                            .Padding(-10)
                            .Background(Colors.Grey.Lighten3)
                            .Padding(10)
                            
                            .Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(16));

                                text.Span(currentException.GetType().Name + ": ").Bold();
                                text.Span(currentException.Message);
                            });
                        
                        foreach (var trace in currentException.StackTrace.Split('\n'))
                        {
                            column
                                .Item()
                                .ShowEntire()
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(5)
                                .Text(trace.Trim())
                                .FontSize(12);
                        }
                        
                        currentException = currentException.InnerException;
                    }
                });
            });
        }
    }   
}