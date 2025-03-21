using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class SvgImageExample
    {
        [Test]
        public void ImageSVG()
        {
            RenderingTest
                .Create()
                .PageSize(new PageSize(75f, 92f, Unit.Millimetre))
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container.Svg("pdf-icon.svg");
                });
        }
        
        [Test]
        public void SupportForDifferentUnits()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container.Padding(20).Column(column =>
                    {
                        column.Spacing(20);
                        
                        var sizes = new[]
                        {
                            ("200", "100"), 
                            ("200px", "100px"), 
                            ("200pt", "100pt"), 
                            ("200cm", "100cm"), 
                            ("200mm", "100mm"), 
                            ("200in", "100in"), 
                            ("200pc", "100pc"), 
                            ("100%", "100%")
                        };

                        foreach (var size in sizes)
                        {
                            column.Item().Row(row =>
                            {
                                // normal SVG
                                row.RelativeItem()
                                    .Width(200)
                                    .Height(100)
                                    .Background(Colors.Grey.Lighten2)
                                    .Svg(CreateSvg(size.Item1, size.Item2));
                                
                                // dynamic SVG
                                row.RelativeItem()
                                    .Width(200)
                                    .Height(100)
                                    .Background(Colors.Grey.Lighten2)
                                    .Svg(_ => CreateSvg(size.Item1, size.Item2));
                            });
                        }
                    });
                });

            string CreateSvg(string width, string height)
            {
                var svg = 
                """
                <svg width="{width}" height="{height}" viewBox="0 0 200 100" xmlns="http://www.w3.org/2000/svg">
                   <ellipse cx="100" cy="50" rx="100" ry="50" style="fill:#2196F3;stroke:#3F51B5;stroke-width:3" />
                </svg>
                """;

                return svg
                    .Replace("{width}", width)
                    .Replace("{height}", height);
            }
        }
    }
}