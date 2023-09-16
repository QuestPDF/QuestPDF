using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples
{
    [TestFixture]
    public class ElementExamples
    {
        [Test]
        public void Placeholder()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Placeholder();
                });
        }
        
        [Test]
        public void Decoration()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .Background(Colors.Grey.Medium)
                                .Padding(10)
                                .Text("Notes")
                                .FontSize(16)
                                .FontColor("#FFF");
                    
                            decoration
                                .Content()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .ExtendVertical()
                                .Text(Helpers.Placeholders.LoremIpsum());
                        });
                });
        }

        [Test]
        public void Row()
        {
            RenderingTest
                .Create()
                .PageSize(740, 200)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item()
                                .PaddingBottom(10)
                                .AlignCenter()
                                .Text("This Row element is 700pt wide");

                            column.Item().Row(row =>
                            {
                                row.ConstantItem(100)
                                    .Background(Colors.Grey.Lighten1)
                                    .Padding(10)
                                    .ExtendVertical()
                                    .Text("This column is 100 pt wide");

                                row.RelativeItem()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Text("This column takes 1/3 of the available space (200pt)");

                                row.RelativeItem(2)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(10)
                                    .Text("This column takes 2/3 of the available space (400pt)");
                            });
                        });
                });
        }
        
        [Test]
        public void RowSpacing()
        {
            RenderingTest
                .Create()
                .PageSize(740, 200)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(20)
                        .Row(row =>
                        {
                            row.Spacing(20);
                            row.RelativeItem(2).Border(1).Background(Colors.Grey.Lighten1);
                            row.RelativeItem(3).Border(1).Background(Colors.Grey.Lighten2);
                            row.RelativeItem(4).Border(1).Background(Colors.Grey.Lighten3);
                        });
                });
        }
    
        [Test]
        public void column()
        {
            RenderingTest
                .Create()
                .PageSize(500, 360)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(15)
                        .Column(column =>
                        {
                            column.Spacing(15);
                    
                            column.Item().Background(Colors.Grey.Medium).Height(50);
                            column.Item().Background(Colors.Grey.Lighten1).Height(100);
                            column.Item().Background(Colors.Grey.Lighten2).Height(150);
                        });
                });
        }
        
        [Test]
        public void Debug()
        {
            RenderingTest
                .Create()
                .PageSize(210, 210)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .DebugArea("Grid example", Colors.Blue.Medium)
                        .Grid(grid =>
                        {
                            grid.Columns(3);
                            grid.Spacing(5);

                            foreach (var _ in Enumerable.Range(0, 8))
                                grid.Item().Height(50).Placeholder();
                        });
                });
        }
        
        [Test]
        public void ElementEnd()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .Render(container =>
                {
                    var text = "";
            
                    container
                        .Padding(10)
                        .Element(x =>
                        {
                            if (string.IsNullOrWhiteSpace(text))
                                x.Height(10).Width(50).Background("#DDD");
                            else
                                x.Text(text);
                        });
                });
        }
        
        [Test]
        public void GridExample()
        {
            RenderingTest
                .Create()
                .PageSize(400, 230)
                .Render(container =>
                {
                    var textStyle = TextStyle.Default.Size(14);
            
                    container
                        .Padding(15)
                        .AlignRight()
                        .Grid(grid =>
                        {
                            grid.VerticalSpacing(10);
                            grid.HorizontalSpacing(10);
                            grid.AlignCenter();
                            grid.Columns(10); // 12 by default

                            grid.Item(6).Background(Colors.Blue.Lighten1).Height(50);
                            grid.Item(4).Background(Colors.Blue.Lighten3).Height(50);
                    
                            grid.Item(2).Background(Colors.Teal.Lighten1).Height(70);
                            grid.Item(3).Background(Colors.Teal.Lighten2).Height(70);
                            grid.Item(5).Background(Colors.Teal.Lighten3).Height(70);
                    
                            grid.Item(2).Background(Colors.Green.Lighten1).Height(50);
                            grid.Item(2).Background(Colors.Green.Lighten2).Height(50);
                            grid.Item(2).Background(Colors.Green.Lighten3).Height(50);
                        });
                });
        }
        
        [Test]
        public void Canvas()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Canvas((canvas, size) =>
                        {
                            using var paint = new SKPaint
                            {
                                Color = SKColors.Red,
                                StrokeWidth = 10,
                                IsStroke = true
                            };
                        
                            // move origin to the center of the available space
                            canvas.Translate(size.Width / 2, size.Height / 2);
                    
                            // draw a circle
                            canvas.DrawCircle(0, 0, 50, paint);
                        });
                });
        }
 
        [Test]
        public void LayersExample()
        {
            RenderingTest
                .Create()
                .PageSize(400, 250)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Layers(layers =>
                        {
                            // layer below main content
                            layers
                                .Layer()
                                .Height(100)
                                .Width(100)
                                .Background(Colors.Grey.Lighten3);

                            layers
                                .PrimaryLayer()
                                .Padding(25)
                                .Column(column =>
                                {
                                    column.Spacing(5);
                            
                                    foreach (var _ in Enumerable.Range(0, 7))
                                        column.Item().Text(Placeholders.Sentence());
                                });
                        
                            // layer above the main content    
                            layers
                                .Layer()
                                .AlignCenter()
                                .AlignMiddle()
                                .Text("Watermark")
                                .FontSize(48)
                                .Bold()
                                .FontColor(Colors.Green.Lighten3);

                            layers
                                .Layer()
                                .AlignBottom()
                                .Text(text => text.CurrentPageNumber().FontSize(16).FontColor(Colors.Green.Medium));
                        });
                });
        }

        // [Test]
        // public void EnsureSpace()
        // {
        //     RenderingTest
        //         .Create()
        //         .PageSize(300, 400)
        //         .Render(container =>
        //         {
        //             container
        //                 .Padding(50)
        //                 .Page(page =>
        //                 {
        //                     page.Header().PageNumber("Page {pdf:currentPage}");
        //             
        //                     page.Content().Height(300).column(content =>
        //                     {
        //                         content.Item().Height(200).Background(Colors.Grey.Lighten2);
        //                 
        //                         content.Item().EnsureSpace(100).column(column =>
        //                         {
        //                             column.Spacing(10);
        //                     
        //                             foreach (var _ in Enumerable.Range(0, 4))
        //                                 column.Item().Height(50).Background(Colors.Green.Lighten1);
        //                         }); 
        //                     });
        //                 });
        //         });
        // }

        [Test]
        public void RandomColorMatrix()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Grid(grid =>
                        {
                            grid.Columns(5);
                    
                            Enumerable
                                .Range(0, 25)
                                .Select(x => Placeholders.BackgroundColor())
                                .ToList()
                                .ForEach(x => grid.Item().Height(50).Background(x));
                        });
                });
        }
    
        [Test]
        public void DefinedColors()
        {
            var colors = new[]
            {
                Colors.Green.Darken4,
                Colors.Green.Darken3,
                Colors.Green.Darken2,
                Colors.Green.Darken1,
                
                Colors.Green.Medium,
                
                Colors.Green.Lighten1,
                Colors.Green.Lighten2,
                Colors.Green.Lighten3,
                Colors.Green.Lighten4,
                Colors.Green.Lighten5,
                
                Colors.Green.Accent1,
                Colors.Green.Accent2,
                Colors.Green.Accent3,
                Colors.Green.Accent4,
            };
            
            RenderingTest
                .Create()
                .PageSize(450, 150)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Height(100)
                        .Row(row =>
                        {
                            foreach (var color in colors)
                                row.RelativeItem().Background(color);
                        });
                });
        }

        [Test]
        public void DefinedFonts()
        {
            var fonts = new[]
            {
                Fonts.Calibri,
                Fonts.Candara,
                Fonts.Arial,
                Fonts.TimesNewRoman,
                Fonts.Consolas,
                Fonts.Tahoma,
                Fonts.Impact,
                Fonts.Trebuchet,
                Fonts.ComicSans
            };
            
            RenderingTest
                .Create()
                .PageSize(500, 175)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Grid(grid =>
                        {
                            grid.Columns(3);

                            foreach (var font in fonts)
                            {
                                grid.Item()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(10)
                                    .Text(font)
                                    .FontFamily(font)
                                    .FontSize(16);
                            }
                        });
                });
        }

        [Test]
        public void Layers()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Layers(layers =>
                        {
                            layers.Layer().Text("Something else");
                    
                            layers.PrimaryLayer().Column(column =>
                            {
                                column.Item().PaddingTop(20).Text("Text 1");
                                column.Item().PaddingTop(40).Text("Text 2");
                            });
                    
                            layers.Layer().Canvas((canvas, size) =>
                            {
                                using var paint = new SKPaint
                                {
                                    Color = SKColors.Red,
                                    StrokeWidth = 5
                                };
                        
                                canvas.Translate(size.Width / 2, size.Height / 2);
                                canvas.DrawCircle(0, 0, 50, paint);
                            });
                    
                            layers.Layer().Background("#8F00").Extend();
                            layers.Layer().PaddingTop(40).Text("It works!").FontSize(24);
                        });
                });
        }

        [Test]
        public void Box()
        {
            RenderingTest
                .Create()
                .PageSize(300, 150)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(15)
                        .Border(4)
                        .BorderColor(Colors.Blue.Medium)
                        //.MinimalBox()
                        .Background(Colors.Grey.Lighten2)
                        .Padding(15)
                        .Text("Test of the \n box element").FontSize(20);
                });
        }

        [Test]
        public void Scale()
        {
            RenderingTest
                .Create()
                .PageSize(300, 175)
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(10)
                        .Decoration(decoration =>
                        {
                            var headerFontStyle = TextStyle
                                .Default
                                .Size(20)
                                .Color(Colors.Blue.Darken2)
                                .SemiBold();
    
                            decoration
                                .Before()
                                .PaddingBottom(10)
                                .Text("Example: scale component")
                                .Style(headerFontStyle);
    
                            decoration
                                .Content()
                                .Column(column =>
                                {
                                    var scales = new[] { 0.8f, 0.9f, 1.1f, 1.2f };

                                    foreach (var scale in scales)
                                    {
                                        var fontColor = scale <= 1f
                                            ? Colors.Red.Lighten4
                                            : Colors.Green.Lighten4;

                                        var fontStyle = TextStyle.Default.Size(16);
                
                                        column
                                            .Item()
                                            .Border(1)
                                            .Background(fontColor)
                                            .Scale(scale)
                                            .Padding(5)
                                            .Text($"Content with {scale} scale.")
                                            .Style(fontStyle);
                                    }
                                });
                        });
                });
        }

        [Test]
        public void Translate()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .MinimalBox()
                        
                        .Padding(25)
                        
                        .Background(Colors.Green.Lighten3)
                        
                        .TranslateX(15)
                        .TranslateY(15)
                        
                        .Border(2)
                        .BorderColor(Colors.Green.Darken1)
                        
                        .Padding(50)
                        .Text("Moved text")
                        .FontSize(25);
                });
        }

        [Test]
        public void ConstrainedRotate()
        {
            RenderingTest
                .Create()
                .PageSize(650, 450)
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Spacing(10);
                            
                            foreach (var turns in Enumerable.Range(0, 4))
                            {
                                grid.Item()
                                    .Width(300)
                                    .Height(200)
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Element(element =>
                                    {
                                        foreach (var x in Enumerable.Range(0, turns))
                                            element = element.RotateRight();

                                        return element;
                                    })
                                    .MinimalBox()
                                    .Background(Colors.White)
                                    .Padding(10)
                                    .Text($"Rotated {turns * 90}°")
                                    .FontSize(16);
                            }
                        });
                });
        }
        
        [Test]
        public void FreeRotate()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Background(Colors.Grey.Lighten2)
                        
                        .AlignCenter()
                        .AlignMiddle()
                        
                        
                        .Background(Colors.White)
                        
                        .Rotate(30)

                        .Width(100)
                        .Height(100)
                        .Background(Colors.Blue.Medium);
                });
        }
        
        [Test]
        public void FreeRotateCenter()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Background(Colors.Grey.Lighten2)
                        
                        .AlignCenter()
                        .AlignMiddle()
                        
                        .Background(Colors.White)
                        
                        .TranslateX(50)
                        .TranslateY(50)
                        
                        .Rotate(30)

                        .TranslateX(-50)
                        .TranslateY(-50)
                        
                        .Width(100)
                        .Height(100)
                        .Background(Colors.Blue.Medium);
                });
        }

        [Test]
        public void Flip()
        {
            RenderingTest
                .Create()
                .PageSize(350, 350)
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Spacing(10);
                            
                            foreach (var turns in Enumerable.Range(0, 4))
                            {
                                grid.Item()
                                    .Width(150)
                                    .Height(150)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(10)
                                    .Element(element =>
                                    {
                                        if (turns == 1 || turns == 2)
                                            element = element.FlipHorizontal();

                                        if (turns == 2 || turns == 3)
                                            element = element.FlipVertical();
                                        
                                        return element;
                                    })
                                    .MinimalBox()
                                    .Background(Colors.White)
                                    .Padding(10)
                                    .Text($"Flipped {turns}")
                                    .FontSize(16);
                            }
                        });
                });
        }
        
        [Test]
        public void RotateInTable()
        {
            RenderingTest
                .Create()
                .PageSize(200, 200)
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Border(2)
                        .Row(row =>
                        {
                            row.ConstantItem(25)
                                .Border(1)
                                .RotateLeft()
                                .AlignCenter()
                                .AlignMiddle()
                                .Text("Sample text");
                            
                            row.RelativeItem().Border(1).Padding(5).Text(Placeholders.Paragraph());
                        });
                });
        }
        
        [Test]
        public void Unconstrained()
        {
            RenderingTest
                .Create()
                .PageSize(400, 350)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .PaddingLeft(50)
                        .Column(column =>
                        {
                            column.Item().Width(300).Height(150).Background(Colors.Blue.Lighten4);
                            
                            column
                                .Item()
                                
                                // creates an infinite space for its child
                                .Unconstrained()
                                
                                // moves the child up and left
                                .TranslateX(-50)
                                .TranslateY(-50)
                                
                                // limits the space for the child
                                .Width(100)
                                .Height(100)
                                
                                .Background(Colors.Blue.Darken1);
                            
                            column.Item().Width(300).Height(150).Background(Colors.Blue.Lighten3);
                        });
                });
        }
        
        [Test]
        public void ComplexLayout()
        {
            RenderingTest
                .Create()
                .PageSize(500, 225)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().LabelCell("Label 1");
                                
                                row.RelativeItem(3).Grid(grid =>
                                {
                                    grid.Columns(3);
                                    
                                    grid.Item(2).LabelCell("Label 2");
                                    grid.Item().LabelCell("Label 3");
                                    
                                    grid.Item(2).ValueCell().Text("Value 2");
                                    grid.Item().ValueCell().Text("Value 3");
                                });
                            });
                            
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().ValueCell().Text("Value 1");
                                
                                row.RelativeItem(3).Grid(grid =>
                                {
                                    grid.Columns(3);
                                    
                                    grid.Item().LabelCell("Label 4");
                                    grid.Item(2).LabelCell("Label 5");
                                    
                                    grid.Item().ValueCell().Text("Value 4");
                                    grid.Item(2).ValueCell().Text("Value 5");
                                });
                            });
                            
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().LabelCell("Label 6");
                                row.RelativeItem().ValueCell().Text("Value 6");
                            });
                        });
                });
        }
        
        [Test]
        public void DomainSpecificLanguage()
        {
            RenderingTest
                .Create()
                .PageSize(600, 310)
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Grid(grid =>
                        {
                            grid.Columns(10);
                            
                            for(var i=1; i<=4; i++)
                            {
                                grid.Item(2).LabelCell(Placeholders.Label());
                                grid.Item(3).ValueCell().Image(Placeholders.Image(200, 150));
                            }
                        });
                });
        }
        
        [Test]
        public void DrawOverflow()
        {
            RenderingTest
                .Create()
                .ShowResults()
                .ProducePdf()
                .RenderDocument(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(24);
                        page.DefaultTextStyle(TextStyle.Default.FontSize(16));

                        page.Header().Text("Document header").FontSize(24).Bold().FontColor(Colors.Blue.Accent2);

                        page.Content().PaddingVertical(24).Column(column =>
                        {
                            column.Spacing(16);

                            foreach (var size in Enumerable.Range(20, 20))
                                column.Item().Width(size * 10).Height(40).Background(Colors.Grey.Lighten3);
                            
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Will it work?").FontSize(20);
                                row.RelativeItem().Border(1).Background(Colors.Grey.Lighten3).Padding(5).ContentOverflowDebugArea().ShowEntire().Text(Placeholders.LoremIpsum()).FontSize(20);
                            });
                        });
                        
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}
