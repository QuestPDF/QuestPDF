namespace QuestPDF.LayoutTests;

/// <summary>
/// Layout coverage for the Inlined element.
/// Expectations were captured from the implementation as of commit b18dde1d, i.e. before the
/// allocation-focused rewrite of Inlined.Compose. The one exception is
/// <see cref="PartiallyRenderedChildIsRejected"/>, which encodes the behaviour introduced by that
/// rewrite because it supersedes a silent content-loss defect.
/// </summary>
public class InlinedTests
{
    [Test]
    public void DefaultAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("c").Position(200, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void CenterAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.AlignCenter();

                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the required area covers the content only, not the alignment padding
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(250, 0).Size(100, 100);
                        page.Mock("b").Position(350, 0).Size(100, 100);
                        page.Mock("c").Position(450, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void RightAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.AlignRight();

                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(500, 0).Size(100, 100);
                        page.Mock("b").Position(600, 0).Size(100, 100);
                        page.Mock("c").Position(700, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void JustifyAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.AlignJustify();

                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(350, 0).Size(100, 100);
                        page.Mock("c").Position(700, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void SpaceAroundAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.AlignSpaceAround();

                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(125, 0).Size(100, 100);
                        page.Mock("b").Position(350, 0).Size(100, 100);
                        page.Mock("c").Position(575, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void RightToLeftDefaultAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.ContentFromRightToLeft().Inlined(inlined =>
                {
                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(300, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(700, 0).Size(100, 100);
                        page.Mock("b").Position(600, 0).Size(100, 100);
                        page.Mock("c").Position(500, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void MultipleLinesWithSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(350, 500)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.HorizontalSpacing(13);
                    inlined.VerticalSpacing(7);

                    inlined.Item().Mock("a").SolidBlock(100, 30);
                    inlined.Item().Mock("b").SolidBlock(100, 40);
                    inlined.Item().Mock("c").SolidBlock(100, 50);
                    inlined.Item().Mock("d").SolidBlock(100, 60);
                    inlined.Item().Mock("e").SolidBlock(100, 70);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(326, 127)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 30);
                        page.Mock("b").Position(113, 0).Size(100, 40);
                        page.Mock("c").Position(226, 0).Size(100, 50);

                        page.Mock("d").Position(0, 57).Size(100, 60);
                        page.Mock("e").Position(113, 57).Size(100, 70);
                    });
            });
    }

    [Test]
    public void LineNotFittingVerticallyIsMovedToTheNextPage()
    {
        LayoutTest
            .HavingSpaceOfSize(250, 120)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 100);
                    inlined.Item().Mock("c").SolidBlock(100, 100);
                    inlined.Item().Mock("d").SolidBlock(100, 100);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                    });

                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("c").Position(0, 0).Size(100, 100);
                        page.Mock("d").Position(100, 0).Size(100, 100);
                    });
            });
    }

    [Test]
    public void ZeroHeightItemStretchesToLineHeight()
    {
        LayoutTest
            .HavingSpaceOfSize(400, 200)
            .ForContent(content =>
            {
                content.Inlined(inlined =>
                {
                    inlined.BaselineBottom();

                    inlined.Item().Mock("a").SolidBlock(100, 100);
                    inlined.Item().Mock("b").SolidBlock(100, 0);
                });
            })
            .ExpectDrawResult(document =>
            {
                // known quirk: the zero-height item is stretched to the line height and then also pushed
                // down by the baseline offset computed from its original height, so it paints below the
                // reported area. The reported area covers the line height, as it always has.
                document
                    .Page()
                    .RequiredAreaSize(200, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 100).Size(100, 100);
                    });
            });
    }

    [Test]
    public void PartiallyRenderedChildIsRejected()
    {
        // Inlined consumes every item it places, so it cannot continue a partially rendered child
        // on the next page. Accepting one silently drops its remainder, so such an item is rejected.
        LayoutTest
            .HavingSpaceOfSize(300, 500)
            .ForContent(content =>
            {
                content.Inlined(outer =>
                {
                    outer.Item().Mock("outer").Inlined(inner =>
                    {
                        inner.Item().Mock("a").SolidBlock(100, 100);
                        inner.Item().Mock("b").SolidBlock(400, 100);
                    });
                });
            })
            .ExpectLayoutException("The available space is not sufficient to fully render even a single item.");
    }

    [Test]
    public void InsideRowAutoItemWithCenterAlignment()
    {
        LayoutTest
            .HavingSpaceOfSize(800, 500)
            .ForContent(content =>
            {
                content.Row(row =>
                {
                    row.AutoItem().Mock("inlined").Inlined(inlined =>
                    {
                        inlined.AlignCenter();

                        inlined.Item().Mock("a").SolidBlock(100, 100);
                        inlined.Item().Mock("b").SolidBlock(100, 100);
                    });

                    row.RelativeItem().Mock("filler").SolidBlock(10, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the auto item takes the content width, leaving the rest of the row to the filler
                document
                    .Page()
                    .RequiredAreaSize(800, 100)
                    .Content(page =>
                    {
                        page.Mock("inlined").Position(0, 0).Size(200, 500);
                        page.Mock("a").Position(0, 0).Size(100, 100);
                        page.Mock("b").Position(100, 0).Size(100, 100);
                        page.Mock("filler").Position(200, 0).Size(600, 500);
                    });
            });
    }
}
