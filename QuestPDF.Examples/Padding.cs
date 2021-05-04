using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class Examples : ExampleTestBase
    {
        public void Padding(IContainer container)
        {
            container
                .Background("#FDD")
                .Padding(50)

                .Background("#AFA")
                .PaddingVertical(50)

                .Background("#77F")
                .PaddingHorizontal(50)

                .Background("#444");
        }
        
        public void Border(IContainer container)
        {
            container
                .Background("#EEE")
                .Padding(25)

                .AlignBottom()
                .AlignCenter()
                .BorderBottom(2)
                .BorderColor("#000")
                
                .Background("FFF")
                .Padding(5)
                .Text("Sample text", TextStyle.Default.FontType("Segoe UI emoji").Alignment(HorizontalAlignment.Center));
        }
        
        public void Alignment(IContainer container)
        {
            container
                .Stack(column =>
                {
                    column
                        .Item()
                        .Height(100)
                        .Background("#FFF")
                        
                        .AlignLeft()
                        .AlignMiddle()

                        .Width(50)
                        .Height(50)
                        .Background("#444");
                    
                    column
                        .Item()
                        .Height(100)
                        .Background("#DDD")
                        
                        .AlignCenter()
                        .AlignMiddle()

                        .Width(50)
                        .Height(50)
                        .Background("#222");
                    
                    column
                        .Item()
                        .Height(100)
                        .Background("#BBB")
                        
                        .AlignRight()
                        .AlignMiddle()

                        .Width(50)
                        .Height(50)
                        .Background("#000");
                });
        }
        
        public void Expand(IContainer container)
        {
            container
                .Stack(column =>
                {
                    column
                        .Item()
                        .Height(150)
                        .Row(row =>
                        {
                            row.RelativeColumn()
                                .Extend()
                                .Background("FFF")

                                .Height(50)
                                .Width(50)
                                .Background("444");
                            
                            row.RelativeColumn()
                                .Extend()
                                .Background("BBB")

                                .Height(50)
                                .ExtendHorizontal()
                                .Background("444");
                        });
                    
                    column
                        .Item()
                        .Height(150)
                        .Row(row =>
                        {
                            row.RelativeColumn()
                                .Extend()
                                .Background("BBB")

                                .ExtendVertical()
                                .Width(50)
                                .Background("444");
                            
                            row.RelativeColumn()
                                .Extend()
                                .Background("BBB")

                                .ExtendVertical()
                                .ExtendHorizontal()
                                .Background("444");
                        });
                });
        }
    }
}