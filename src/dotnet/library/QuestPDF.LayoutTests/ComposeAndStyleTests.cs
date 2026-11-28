using QuestPDF.Elements;

namespace QuestPDF.LayoutTests;

public class ComposeAndStyleTests
{
    #region Compose

    [Test]
    public void ContentComposedByMethodsIsPlacedWhereComposeWasCalled()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Compose(ContentA);
                    column.Item().Compose(ContentB);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(150, 120)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(150, 20);
                        page.Mock("b").Position(0, 20).Size(150, 100);
                    });
            });

        static void ContentA(IContainer container) => container.Mock("a").SolidBlock(100, 20);
        static void ContentB(IContainer container) => container.Mock("b").SolidBlock(150, 100);
    }

    [Test]
    public void MethodIsReusedWithDifferentArguments()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.AutoItem().Compose(x => TestItem(x, "a"));
                    row.AutoItem().Compose(x => TestItem(x, "b"));
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(160, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(80, 40);
                        page.Mock("b").Position(80, 0).Size(80, 40);
                    });
            });

        static void TestItem(IContainer container, string party) => container.Mock(party).SolidBlock(80, 40);
    }

    [Test]
    public void MethodThatComposesNothingTakesNoSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("before").SolidBlock(50, 20);
                    column.Item().Compose(EmptyItem);
                    column.Item().Mock("after").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 40)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("after").Position(0, 20).Size(50, 20);
                    });
            });

        static void EmptyItem(IContainer container) { }
    }

    [Test]
    public void LayoutExceptionPointsToMethodThatComposedProblematicContent()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Compose(TooWideContent);
            })
            .ExpectLayoutException($"In method:   {nameof(TooWideContent)}");

        static void TooWideContent(IContainer container) => container.Width(200).SolidBlock(200, 50);
    }

    #endregion

    #region Component

    private class TooWideComponent : IComponent
    {
        public void Compose(IContainer container) => container.Width(200).SolidBlock(200, 50);
    }

    [Test]
    public void LayoutExceptionPointsToComponentThatComposedProblematicContent()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Component(new TooWideComponent());
            })
            .ExpectLayoutException($"-> {nameof(TooWideComponent)}");
    }

    #endregion

    #region Style

    [Test]
    public void StyleIsSharedByManyElements()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Row(row =>
                {
                    row.AutoItem().Style(ItemPadding).Mock("a").SolidBlock(60, 20);
                    row.AutoItem().Style(ItemPadding).Mock("b").SolidBlock(40, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(140, 40)
                    .Content(page =>
                    {
                        page.Mock("a").Position(10, 10).Size(60, 20);
                        page.Mock("b").Position(90, 10).Size(40, 20);
                    });
            });

        static IContainer ItemPadding(IContainer container) => container.Padding(10);
    }

    [Test]
    public void StylesAreNestedInOrderOfCalls()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink()
                    .Style(ItemSize) // <-
                    .Style(ItemPadding) // <-
                    .Mock("content")
                    .SolidBlock(80, 40);
            })
            .ExpectDrawResult(document =>
            {
                // the padding is applied inside the card: in the opposite order, it would surround the card and the page would take 120 x 80
                document
                    .Page()
                    .RequiredAreaSize(100, 60)
                    .Content(page =>
                    {
                        page.Mock("content").Position(10, 10).Size(80, 40);
                    });
            });

        static IContainer ItemSize(IContainer container) => container.Width(100).Height(60);
        static IContainer ItemPadding(IContainer container) => container.Padding(10);
    }

    [Test]
    public void StyledContentComposedByMethodContinuesOnNextPages()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Style(ItemPadding).Compose(LongContent);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 100)
                    .Content(page =>
                    {
                        page.Mock("content").Position(10, 10).Size(50, 80);
                    });

                document
                    .Page()
                    .RequiredAreaSize(70, 90)
                    .Content(page =>
                    {
                        page.Mock("content").Position(10, 10).Size(50, 70);
                    });
            });

        static IContainer ItemPadding(IContainer container) => container.Padding(10);
        static void LongContent(IContainer container) => container.Mock("content").ContinuousBlock(50, 150);
    }

    #endregion

    #region StyleIf

    [Test]
    public void StyleIfStylesOnlyElementsThatMeetTheCondition()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().StyleIf(false, ApplyIndent).Mock("regular").SolidBlock(50, 10);
                    column.Item().StyleIf(true, ApplyIndent).Mock("indented").SolidBlock(50, 10);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 20)
                    .Content(page =>
                    {
                        page.Mock("regular").Position(0, 0).Size(70, 10);
                        page.Mock("indented").Position(20, 10).Size(50, 10);
                    });
            });

        static IContainer ApplyIndent(IContainer container) => container.PaddingLeft(20);
    }

    #endregion

    #region Element

    [Test]
    public void ContentCreatedInAdvanceIsPlacedWhereElementWasCalled()
    {
        // the summary is defined before the document, when its final position is not known yet
        var summary = EmptyContainer.Create();
        summary.Mock("summary").SolidBlock(80, 30);

        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Padding(10).Element(summary);
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("summary").Position(10, 10).Size(80, 30);
                    });
            });
    }

    private class HeaderAndBodyComponent : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(x => x.SolidBlock(120, 45));

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }

    [Test]
    public void DynamicComponentArrangesElementsItCreatedEarlier()
    {
        LayoutTest
            .HavingSpaceOfSize(200, 200)
            .ForContent(content =>
            {
                content.Shrink().Mock("dynamic").Dynamic(new HeaderAndBodyComponent());
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(120, 45)
                    .Content(page =>
                    {
                        page.Mock("dynamic").Position(0, 0).Size(120, 45);
                    });
            });
    }

    #endregion
}
