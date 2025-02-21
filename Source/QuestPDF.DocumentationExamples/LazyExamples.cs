using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class LazyExamples
{
    class SimpleComponent : IComponent
    {
        public required int Start { get; init; }
        public required int End { get; init; }
        
        public void Compose(IContainer container)
        {
            container.Decoration(decoration =>
            {
                decoration.Before()
                    .Text($"Numbers from {Start} to {End}")
                    .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
            
                decoration.Content().Column(column =>
                {
                    foreach (var i in Enumerable.Range(Start, End - Start + 1))
                        column.Item().Text($"Number {i}").FontSize(10);
                });
            });
        }
    }

    [Test]
    public void Disabled()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(10);

                    page.Content().Column(column =>
                    {
                        const int sectionSize = 1000;
                        
                        foreach (var i in Enumerable.Range(0, 1000))
                        {
                            column.Item().Component(new SimpleComponent
                            {
                                Start = i * sectionSize,
                                End = i * sectionSize + sectionSize - 1
                            });
                        }
                    });
                });
            })
            .GeneratePdf("lazy-disabled.pdf");
    }

    [Test]
    public void Enabled()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(10);
                    page.Content().Column(column =>
                    {
                        const int sectionSize = 1000;

                        foreach (var i in Enumerable.Range(0, 1000))
                        {
                            var start = i * sectionSize;
                            var end = start + sectionSize - 1;

                            column.Item().Lazy(c =>
                            {
                                c.Component(new SimpleComponent
                                {
                                    Start = start,
                                    End = end
                                });
                            });
                        }
                    });
                });
            })
            .GeneratePdf("lazy-enabled.pdf");
    }
    
    [Test]
    public void EnabledWithCache()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(10);

                    page.Content().Column(column =>
                    {
                        const int sectionSize = 1000;

                        foreach (var i in Enumerable.Range(0, 1000))
                        {
                            var start = i * sectionSize;
                            var end = start + sectionSize - 1;

                            column.Item().LazyWithCache(c =>
                            {
                                c.Component(new SimpleComponent
                                {
                                    Start = start,
                                    End = end
                                });
                            });
                        }
                    });
                });
            })
            .GeneratePdf("lazy-enabled-with-cache.pdf");
    }
}