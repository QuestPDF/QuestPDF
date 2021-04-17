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
        public void Section(IContainer container)
        {
            container
                .Background("#FFF")
                .Padding(25)
                .Section(section =>
                {
                    section
                        .Header()
                        .Background("#888")
                        .Padding(10)
                        .Text("Notes", TextStyle.Default.Size(16).Color("#FFF"));
                    
                    section
                        .Content()
                        .Background("#DDD")
                        .Padding(10)
                        .ExtendVertical()
                        .Text(TextPlaceholder.LoremIpsum());
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
                    stack.Element()
                        .PaddingBottom(10)
                        .AlignCenter()
                        .Text("This Row element is 700pt wide");

                    stack.Element().Row(row =>
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
                        .Element()
                        .Background("#999")
                        .Height(50);
                    
                    column
                        .Element()
                        .Background("#BBB")
                        .Height(100);
                    
                    column
                        .Element()
                        .Background("#DDD")
                        .Height(150);
                });
        }
        
        //[ShowResult]
        [ImageSize(300, 200)]
        public void Debug(IContainer container)
        {
            container
                .Padding(25)
                .Debug()
                .Padding(-5)
                .Row(row =>
                {
                    row.RelativeColumn().Padding(5).Extend().Placeholder();
                    row.RelativeColumn().Padding(5).Extend().Placeholder();
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
                    grid.Spacing(5);
                    grid.Columns(12);

                    grid.Element(8).Background("#DDD").Height(50).Padding(5).Text("This is a short text", textStyle);
                    grid.Element(4).Background("#BBB").Padding(5).Text("Showing how to...", textStyle);
                    grid.Element(2).Background("#999").Height(50);
                    grid.Element(4).Background("#AAA").Border(2).BorderColor("#666").Padding(5).Text("...generate", textStyle);
                    grid.Element(6).Background("#CCC").Padding(5).Text("simple grids", textStyle.Size(18).Bold());
                    grid.Element(8).Background("#DDD").Height(50);
                });
        }
        
        [ShowResult]
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
                        stack.Element().PaddingTop(20).Text("Dupa 1");
                        stack.Element().PaddingTop(40).Text("Dupa 2");
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
                    layers.Layer().PaddingTop(40).Text("Super", TextStyle.Default.Size(24));
                });
        }
    }
}