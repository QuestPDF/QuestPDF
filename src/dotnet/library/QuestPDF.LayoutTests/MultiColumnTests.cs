using QuestPDF.Elements;
using QuestPDF.Helpers;

namespace QuestPDF.LayoutTests;

public class MultiColumnTests
{
    [Test]
    public void DynamicComponent()
    {
        LayoutTest
            .HavingSpaceOfSize(400, 200)
            .ForContent(content =>
            {
                content
                    .Shrink()
                    .MultiColumn(column =>
                    {
                        column.Content()
                            .Mock("dynamic")
                            .Dynamic(new CounterComponent());
                    });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(400, 50)
                    .Content(page =>
                    {
                        page.Mock("dynamic")
                            .Position(0, 0)
                            .Size(200, 50)
                            .State(new DynamicHost.DynamicState()
                            {
                                IsRendered = false,
                                RenderCount = 1,
                                ChildState = 2
                            });
                        
                        page.Mock("dynamic")
                            .Position(200, 0)
                            .Size(200, 50)
                            .State(new DynamicHost.DynamicState()
                            {
                                IsRendered = false,
                                RenderCount = 2,
                                ChildState = 3
                            });
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(400, 50)
                    .Content(page =>
                    {
                        page.Mock("dynamic")
                            .Position(0, 0)
                            .Size(200, 50)
                            .State(new DynamicHost.DynamicState()
                            {
                                IsRendered = false,
                                RenderCount = 3,
                                ChildState = 4
                            });
                        
                        page.Mock("dynamic")
                            .Position(200, 0)
                            .Size(200, 50)
                            .State(new DynamicHost.DynamicState()
                            {
                                IsRendered = false,
                                RenderCount = 4,
                                ChildState = 5
                            });
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(400, 50)
                    .Content(page =>
                    {
                        page.Mock("dynamic")
                            .Position(0, 0)
                            .Size(200, 50)
                            .State(new DynamicHost.DynamicState()
                            {
                                IsRendered = true,
                                RenderCount = 5,
                                ChildState = 6
                            });
                    });
            });
    }
    
    public class CounterComponent : IDynamicComponent<int>
    {
        public int State { get; set; } = 1;
        
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(element =>
            {
                element
                    .Width(100)
                    .Height(50)
                    .Background(Colors.Grey.Lighten2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text($"Item {State}")
                    .SemiBold();
            });

            State++;

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = State <= 5
            };
        }
    }
}