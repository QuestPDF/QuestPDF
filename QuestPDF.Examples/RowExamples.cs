using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class RowExamples
    {
        [Test]
        public void ItemTypes()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(650, 300)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .MinimalBox()
                        .Border(1)
                        .Column(column =>
                        {
                            column.Item().LabelCell("Total width: 600px");
                            
                            column.Item().Row(row =>
                            {
                                row.ConstantItem(150).ValueCell("150px");
                                row.ConstantItem(100).ValueCell("100px");
                                row.RelativeItem(4).ValueCell("200px");
                                row.RelativeItem(3).ValueCell("150px");
                            });
                            
                            column.Item().Row(row =>
                            {
                                row.ConstantItem(100).ValueCell("100px");
                                row.ConstantItem(50).ValueCell("50px");
                                row.RelativeItem(2).ValueCell("100px");
                                row.RelativeItem(1).ValueCell("50px");
                            });
                        });
                });
        }
        
        [Test]
        public void Stability()
        {
            // up to version 2021.12, this code would always result with the infinite layout exception
            
            RenderingTest
                .Create()
                .ProducePdf()
                .MaxPages(100)
                .PageSize(250, 150)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().ShowOnce().Element(CreateBox).Text("X");
                                column.Item().Element(CreateBox).Text("1");
                                column.Item().Element(CreateBox).Text("2");
                            });
                                
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Element(CreateBox).Text("1");
                                column.Item().Element(CreateBox).Text("2");
                            });
                        });
                });

            static IContainer CreateBox(IContainer container)
            {
                return container
                    .ExtendHorizontal()
                    .ExtendVertical()
                    .Background(Colors.Grey.Lighten4)
                    .Border(1)
                    .AlignCenter()
                    .AlignMiddle()
                    .ShowOnce();
            }
        }
    }
}