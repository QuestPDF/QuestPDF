using System.Linq;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples
{
    public class ElementExamples : ExampleTestBase
    {
        //[ShowResult]
        [ImageSize(200, 150)]
        public void Placeholder(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(25)
                .Placeholder();
        }
        
        //[ShowResult]
        [ImageSize(300, 300)]
        public void Decoration(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(25)
                .Decoration(decoration =>
                {
                    decoration
                        .Header()
                        .Background("#888")
                        .Padding(10)
                        .Text("Notes", TextStyle.Default.Size(16).Color("#FFF"));
                    
                    decoration
                        .Content()
                        .Background("#DDD")
                        .Padding(10)
                        .ExtendVertical()
                        .Text(Helpers.Placeholders.LoremIpsum());
                });
        }
        
        //[ShowResult]
        [ImageSize(298, 421)]
        public void Page(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(15)
                .Page(page =>
                {
                    page.Header()
                        .Height(60)
                        .Background("#BBB")
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Header");
                    
                    page.Content()
                        .Background("#DDD")
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Content");
                        
                    page.Footer()
                        .Height(30)
                        .Background("#BBB")
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Footer");
                });
        }
        
        //[ShowResult]
        [ImageSize(740, 200)]
        public void Row(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(20)
                .Stack(stack =>
                {
                    stack.Item()
                        .PaddingBottom(10)
                        .AlignCenter()
                        .Text("This Row element is 700pt wide");

                    stack.Item().Row(row =>
                    {
                        row.ConstantColumn(100)
                            .Background("#DDD")
                            .Padding(10)
                            .ExtendVertical()
                            .Text("This column is 100 pt wide");

                        row.RelativeColumn()
                            .Background("#BBB")
                            .Padding(10)
                            .Text("This column takes 1/3 of the available space (200pt)");

                        row.RelativeColumn(2)
                            .Background("#DDD")
                            .Padding(10)
                            .Text("This column takes 2/3 of the available space (400pt)");
                    });
                });
        }
        
        //[ShowResult]
        [ImageSize(500, 350)]
        public void Column(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(15)
                .Stack(column =>
                {
                    column.Spacing(10);
                    
                    column
                        .Item()
                        .Background("#999")
                        .Height(50);
                    
                    column
                        .Item()
                        .Background("#BBB")
                        .Height(100);
                    
                    column
                        .Item()
                        .Background("#DDD")
                        .Height(150);
                });
        }
        
        [ShowResult]
        [ImageSize(210, 210)]
        public void Debug(IContainer container)
        {
            container
                .Padding(25)
                .Debug("Grid example", Colors.Blue.Medium)
                .Grid(grid =>
                {
                    grid.Columns(3);
                    grid.Spacing(5);

                    foreach (var _ in Enumerable.Range(0, 8))
                        grid.Item().Height(50).Placeholder();
                });
        }
        
        //[ShowResult]
        [ImageSize(300, 200)]
        public void ElementEnd(IContainer container)
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
        }
        
        //[ShowResult]
        [ImageSize(300, 200)]
        public void GridExample(IContainer container)
        {
            var textStyle = TextStyle.Default.Size(14);
            
            container
                .Padding(20)
                .AlignRight()
                .Grid(grid =>
                {
                    grid.VerticalSpacing(20);
                    grid.HorizontalSpacing(10);
                    grid.Columns(12);

                    grid.Item(8).Background("#DDD").Height(50).Padding(5).Text("This is a short text", textStyle);
                    grid.Item(4).Background("#BBB").Padding(5).Text("Showing how to...", textStyle);
                    grid.Item(2).Background("#999").Height(50);
                    grid.Item(4).Background("#AAA").Border(2).BorderColor("#666").Padding(5).Text("...generate", textStyle);
                    grid.Item(6).Background("#CCC").Padding(5).Text("simple grids", textStyle.Size(18).Bold());
                    grid.Item(8).Background("#DDD").Height(50);
                });
        }

        //[ShowResult]
        [ImageSize(300, 300)]
        public void RandomColorMatrix(IContainer container)
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
        }
        
        //[ShowResult]
        [ImageSize(450, 150)]
        public void DefinedColors(IContainer container)
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
            
            container
                .Padding(25)
                .Height(100)
                .Row(row =>
                {
                    foreach (var color in colors)
                        row.RelativeColumn().Background(color);
                });
        }
        
        //[ShowResult]
        [ImageSize(500, 175)]
        public void DefinedFonts(IContainer container)
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
                            .Text(font, TextStyle.Default.FontType(font).Size(16));
                    }
                });
        }
        
        //[ShowResult]
        [ImageSize(300, 300)]
        public void Layers(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(25)
                .Layers(layers =>
                {
                    layers.Layer().Text("Something else");
                    
                    layers.PrimaryLayer().Stack(stack =>
                    {
                        stack.Item().PaddingTop(20).Text("Text 1");
                        stack.Item().PaddingTop(40).Text("Text 2");
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
                    layers.Layer().PaddingTop(40).Text("It works!", TextStyle.Default.Size(24));
                });
        }
    }
}
