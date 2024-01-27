using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.ReportSample.Layouts
{
    public class TableOfContentsTemplate : IComponent
    {
        private List<ReportSection> Sections { get; }

        public TableOfContentsTemplate(List<ReportSection> sections)
        {
            Sections = sections;
        }
        
        public void Compose(IContainer container)
        {
            container
                .Decoration(decoration =>
                {
                    decoration
                        .Before()
                        .PaddingBottom(5)
                        .Text("Table of contents")
                        .Style(Typography.Headline);

                    decoration.Content().Column(column =>
                    {
                        column.Spacing(5);
                        
                        for (var i = 0; i < Sections.Count; i++)
                            column.Item().Element(c => DrawLink(c, i+1, Sections[i].Title));

                        column.Item().Element(c => DrawLink(c, Sections.Count+1, "Photos"));
                    });
                });
        }

        private static void DrawLink(IContainer container, int number, string locationName)
        {
            container
                .SectionLink(locationName)
                .Row(row =>
                {
                    row.ConstantItem(20).Text($"{number}.");
                    row.AutoItem().Text(locationName);
                    
                    row.RelativeItem()
                        .PaddingHorizontal(2)
                        .AlignBottom()
                        .TranslateY(-3)
                        .Height(3)
                        .SkiaSharpRasterized((canvas, size) =>
                        {
                            using var paint = new SKPaint
                            {
                                StrokeWidth = 1,
                                PathEffect = SKPathEffect.CreateDash(new float[] { 1, 3 }, 0),
                            };
                        
                            canvas.Translate(0, 1);
                            canvas.DrawLine(0, 0, size.Width, 0, paint);
                        });
                    
                    row.AutoItem().Text(text =>
                    {
                        text.BeginPageNumberOfSection(locationName);
                        text.Span(" - ");
                        text.EndPageNumberOfSection(locationName);

                        var lengthStyle = TextStyle.Default.FontColor(Colors.Grey.Medium);
                        
                        text.TotalPagesWithinSection(locationName).Style(lengthStyle).Format(x =>
                        {
                            var formatted = x == 1 ? "1 page long" : $"{x} pages long";
                            return $" ({formatted})";
                        });
                    });
                });
        }
    }
}
