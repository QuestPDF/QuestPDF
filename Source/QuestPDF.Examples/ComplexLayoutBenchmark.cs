using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ComplexLayoutBenchmark
    {
        [Test]
        public void ComplexLayout()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(x => GenerateStructure(x, 12));
        }

        private void GenerateStructure(IContainer container, int level)
        {
            if (level <= 0)
            {
                container.Background(Placeholders.BackgroundColor()).Height(10);
                return;
            }

            level--;

            switch (level % 3)
            {
                case 0:
                    {
                        container
                            .Border(level / 4f)
                            .BorderColor(Colors.Black)
                            .Row(row =>
                            {
                                row.RelativeItem().Element(x => GenerateStructure(x, level));
                                row.RelativeItem().Element(x => GenerateStructure(x, level));
                            });
                        break;
                    }

                default:
                    {
                        container
                            .Border(level / 4f)
                            .BorderColor(Colors.Black)
                            .Column(column =>
                            {
                                column.Item().Element(x => GenerateStructure(x, level));
                                column.Item().Element(x => GenerateStructure(x, level));
                            });
                        break;
                    }
            }
        }
    }
}