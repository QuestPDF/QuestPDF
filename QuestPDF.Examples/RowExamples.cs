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
        public void ColumnTypes()
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
                        .Box()
                        .Border(1)
                        .Stack(stack =>
                        {
                            stack.Item().LabelCell("Total width: 600px");
                            
                            stack.Item().Row(row =>
                            {
                                row.ConstantColumn(150).ValueCell("150px");
                                row.ConstantColumn(100).ValueCell("100px");
                                row.RelativeColumn(4).ValueCell("200px");
                                row.RelativeColumn(3).ValueCell("150px");
                            });
                            
                            stack.Item().Row(row =>
                            {
                                row.ConstantColumn(100).ValueCell("100px");
                                row.ConstantColumn(50).ValueCell("50px");
                                row.Column(constantWidth: 100, relativeWidth: 4).ValueCell("350px");
                                row.RelativeColumn(2).ValueCell("100px");
                                row.RelativeColumn(1).ValueCell("50px");
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
                            row.RelativeColumn().Stack(stack =>
                            {
                                stack.Item().ShowOnce().Element(CreateBox).Text("X");
                                stack.Item().Element(CreateBox).Text("1");
                                stack.Item().Element(CreateBox).Text("2");
                            });
                                
                            row.RelativeColumn().Stack(stack =>
                            {
                                stack.Item().Element(CreateBox).Text("1");
                                stack.Item().Element(CreateBox).Text("2");
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