using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    internal class MyComponent : IComponent
    {
        public ISlot Header { get; set; }
        public ISlot<string> Content { get; set; }

        public void Compose(IContainer container)
        {
            container
                .Column(column =>
                {
                    column.Item().Slot(Header);

                    foreach (var i in Enumerable.Range(1, 10))
                        column.Item().Slot(Content, i.ToString());
                });
        }
    }
    
    public class ComponentExamples
    {
        [Test]
        public void ComplexLayout()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(content =>
                {
                    content
                        .Padding(10)
                        .Border(1)
                        .BorderColor(Colors.Grey.Medium)
                        .Component<MyComponent>(component =>
                        {
                            component
                                .Slot(x => x.Header)
                                .Text("This is my text");

                            component.Slot(x => x.Content, (input, container) =>
                            {
                                container
                                    .Background(Placeholders.BackgroundColor())
                                    .Padding(5)
                                    .Text(input);
                            });
                        });
                });
        }
    }
}