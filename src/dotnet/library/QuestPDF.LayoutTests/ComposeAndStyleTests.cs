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
                    column.Item().Compose(Header);
                    column.Item().Compose(Body);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(150, 120)
                    .Content(page =>
                    {
                        page.Mock("header").Position(0, 0).Size(150, 20);
                        page.Mock("body").Position(0, 20).Size(150, 100);
                    });
            });

        static void Header(IContainer container) => container.Mock("header").SolidBlock(100, 20);
        static void Body(IContainer container) => container.Mock("body").SolidBlock(150, 100);
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
                    row.AutoItem().Compose(x => Address(x, "seller"));
                    row.AutoItem().Compose(x => Address(x, "customer"));
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(160, 40)
                    .Content(page =>
                    {
                        page.Mock("seller").Position(0, 0).Size(80, 40);
                        page.Mock("customer").Position(80, 0).Size(80, 40);
                    });
            });

        static void Address(IContainer container, string party) => container.Mock(party).SolidBlock(80, 40);
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
                    column.Item().Compose(OptionalNotes);
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

        // a typical conditional part of the document: there are no notes to show
        static void OptionalNotes(IContainer container) { }
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
                    row.AutoItem().Style(Cell).Mock("product").SolidBlock(60, 20);
                    row.AutoItem().Style(Cell).Mock("price").SolidBlock(40, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(140, 40)
                    .Content(page =>
                    {
                        page.Mock("product").Position(10, 10).Size(60, 20);
                        page.Mock("price").Position(90, 10).Size(40, 20);
                    });
            });

        static IContainer Cell(IContainer container) => container.Padding(10);
    }

    [Test]
    public void StyledContentComposedByMethodContinuesOnNextPages()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Style(Frame).Compose(LongContent);
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

        static IContainer Frame(IContainer container) => container.Padding(10);
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
                    column.Item().StyleIf(false, Indented).Mock("regular").SolidBlock(50, 10);
                    column.Item().StyleIf(true, Indented).Mock("indented").SolidBlock(50, 10);
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

        static IContainer Indented(IContainer container) => container.PaddingLeft(20);
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
            var header = context.CreateElement(x => x.SolidBlock(100, 20));
            var body = context.CreateElement(x => x.SolidBlock(60, 50));

            var content = context.CreateElement(x => x.Column(column =>
            {
                column.Item().Element(header);
                column.Item().Element(body);
            }));

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
                    .RequiredAreaSize(100, 70)
                    .Content(page =>
                    {
                        page.Mock("dynamic").Position(0, 0).Size(100, 70);
                    });
            });
    }

    #endregion
}
