using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class Examples
    {
        [Test]
        public void Padding()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.Red.Lighten2)
                        .Padding(50)

                        .Background(Colors.Green.Lighten2)
                        .PaddingVertical(50)

                        .Background(Colors.Blue.Lighten2)
                        .PaddingHorizontal(50)

                        .Background(Colors.Grey.Darken2);
                });
        }
        
        [Test]
        public void Border()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .Render(container =>
                {
                    container
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)

                        .AlignBottom()
                        .AlignCenter()
                        .BorderBottom(2)
                        .BorderColor(Colors.Black)
                
                        .Background(Colors.White)
                        .Padding(5)
                        .AlignCenter()
                        .Text("Sample text")
                        .FontFamily("Segoe UI emoji");
                });
        }
        
        [Test]
        public void Alignment()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .Render(container =>
                {
                    container
                        .Column(column =>
                        {
                            column
                                .Item()
                                .Height(100)
                                .Background(Colors.White)
                        
                                .AlignLeft()
                                .AlignMiddle()

                                .Width(50)
                                .Height(50)
                                .Background(Colors.Grey.Darken2);
                    
                            column
                                .Item()
                                .Height(100)
                                .Background(Colors.Grey.Lighten4)
                        
                                .AlignCenter()
                                .AlignMiddle()

                                .Width(50)
                                .Height(50)
                                .Background(Colors.Grey.Darken3);
                    
                            column
                                .Item()
                                .Height(100)
                                .Background(Colors.Grey.Lighten3)
                        
                                .AlignRight()
                                .AlignMiddle()

                                .Width(50)
                                .Height(50)
                                .Background(Colors.Black);
                        });
                });
        }
        
        [Test]
        public void Expand()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .Render(container =>
                {
                    container
                        .Column(column =>
                        {
                            column
                                .Item()
                                .Height(150)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Extend()
                                        .Background(Colors.White)

                                        .Height(50)
                                        .Width(50)
                                        .Background(Colors.Grey.Darken2);
                            
                                    row.RelativeItem()
                                        .Extend()
                                        .Background(Colors.Grey.Lighten3)

                                        .Height(50)
                                        .ExtendHorizontal()
                                        .Background(Colors.Grey.Darken2);
                                });
                    
                            column
                                .Item()
                                .Height(150)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Extend()
                                        .Background(Colors.Grey.Lighten3)

                                        .ExtendVertical()
                                        .Width(50)
                                        .Background(Colors.Grey.Darken2);
                            
                                    row.RelativeItem()
                                        .Extend()
                                        .Background(Colors.Grey.Lighten3)

                                        .ExtendVertical()
                                        .ExtendHorizontal()
                                        .Background(Colors.Grey.Darken2);
                                });
                        });
                });
        }
    }
}