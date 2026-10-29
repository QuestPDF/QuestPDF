namespace QuestPDF.LayoutTests;

public class DecorationTests
{
    [Test]
    public void BeforeAndAfterSlotsTogetherTallerThanAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    // each slot fits on its own, but together (120) they are taller than the available space (100)
                    decoration.Before().SolidBlock(50, 60);
                    decoration.Content().SolidBlock(50, 10);
                    decoration.After().SolidBlock(50, 60);
                });
            })
            .ExpectLayoutException("Decoration slots (before and after) together require more vertical space than available.");
    }

    [Test]
    public void DecorationIsMovedToNextPageWhenBeforeAndAfterSlotsDoNotFitInRemainingSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("a").SolidBlock(50, 70);

                    column.Item().Decoration(decoration =>
                    {
                        // the slots together (40) are taller than the remaining space (30)
                        decoration.Before().Mock("before").SolidBlock(50, 20);
                        decoration.Content().Mock("content").SolidBlock(50, 10);
                        decoration.After().Mock("after").SolidBlock(50, 20);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 70);
                    });

                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 10);
                        page.Mock("after").Position(0, 30).Size(50, 20);
                    });
            });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void SlotsAreDrawnAroundHiddenContent(bool usePredicate)
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 20);

                    // ShowIf hides only the content after it, so the decoration keeps its slots, e.g. a table header without rows
                    column.Item().Decoration(decoration =>
                    {
                        decoration.Before().Mock("before").SolidBlock(50, 10);
                        decoration.Content().Mock("content").Hide(usePredicate).SolidBlock(50, 10);
                        decoration.After().Mock("after").SolidBlock(50, 10);
                    });

                    column.Item().Mock("b").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 80)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 20);
                        page.Mock("before").Position(0, 30).Size(50, 10);
                        page.Mock("content").Position(0, 40).Size(50, 0);
                        page.Mock("after").Position(0, 40).Size(50, 10);
                        page.Mock("b").Position(0, 60).Size(50, 20);
                    });
            });
    }

    [Test]
    public void DecorationIsMovedToNextPageTogetherWithPostponedContent()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("a").SolidBlock(50, 60);

                    column.Item().Decoration(decoration =>
                    {
                        // the content (50) does not fit between the slots in the remaining space (40 - 10 - 10 = 20),
                        // PreventPageBreak moves it to the next page, so the slots must not be drawn alone on the first page
                        decoration.Before().Mock("before").SolidBlock(50, 10);

                        decoration.Content().PreventPageBreak().Column(innerColumn =>
                        {
                            innerColumn.Item().Mock("content1").SolidBlock(50, 20);
                            innerColumn.Item().Mock("content2").SolidBlock(50, 30);
                        });

                        decoration.After().Mock("after").SolidBlock(50, 10);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 60);
                    });

                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 10);
                        page.Mock("content1").Position(0, 10).Size(50, 20);
                        page.Mock("content2").Position(0, 30).Size(50, 30);
                        page.Mock("after").Position(0, 60).Size(50, 10);
                    });
            });
    }

    [Test]
    public void PostponedContentStaysPostponedWhenParentOffersMoreSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("a").SolidBlock(50, 40);

                    // the row is as tall as its first item (60), so it draws the decoration in more space than the decoration takes (none),
                    // the content (55) would fit in that space, but it must stay postponed together with the slots
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(50).SolidBlock(50, 60);

                        row.RelativeItem().Decoration(decoration =>
                        {
                            decoration.Before().Mock("before").SolidBlock(50, 10);

                            decoration.Content().PreventPageBreak().Column(innerColumn =>
                            {
                                innerColumn.Item().Mock("content1").SolidBlock(50, 25);
                                innerColumn.Item().Mock("content2").SolidBlock(50, 30);
                            });
                        });
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 40);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 65)
                    .Content(page =>
                    {
                        page.Mock("before").Position(50, 0).Size(50, 10);
                        page.Mock("content1").Position(50, 10).Size(50, 25);
                        page.Mock("content2").Position(50, 35).Size(50, 30);
                    });
            });
    }

    [Test]
    public void ContentSpansMultiplePagesBetweenSlots()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    // the slots repeat on every page, the content gets the space left between them: 100 - 20 - 30 = 50
                    decoration.Before().Mock("before").SolidBlock(50, 20);
                    decoration.Content().Mock("content").ContinuousBlock(50, 120);
                    decoration.After().Mock("after").SolidBlock(50, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 50);
                        page.Mock("after").Position(0, 70).Size(50, 30);
                    });

                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 50);
                        page.Mock("after").Position(0, 70).Size(50, 30);
                    });

                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 20);
                        page.Mock("after").Position(0, 40).Size(50, 30);
                    });
            });
    }

    [Test]
    public void SlotsExactlyFillAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    // the slots together take all the available space, which still leaves room for content without height
                    decoration.Before().Mock("before").SolidBlock(50, 40);
                    decoration.Content().Mock("content").SolidBlock(50, 0);
                    decoration.After().Mock("after").SolidBlock(50, 60);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 40);
                        page.Mock("content").Position(0, 40).Size(50, 0);
                        page.Mock("after").Position(0, 40).Size(50, 60);
                    });
            });
    }

    [Test]
    public void SlotsDefinedWithHandlers()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    decoration.Before(slot => slot.Mock("before").SolidBlock(50, 20));
                    decoration.Content(slot => slot.Mock("content").SolidBlock(50, 30));
                    decoration.After(slot => slot.Mock("after").SolidBlock(50, 20));
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 30);
                        page.Mock("after").Position(0, 50).Size(50, 20);
                    });
            });
    }

    [Test]
    public void LeftToRightDirection()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // the decoration is as wide as its widest slot, and is placed at the left edge of the available space
                content.Decoration(decoration =>
                {
                    decoration.Before().Mock("before").SolidBlock(50, 20);
                    decoration.Content().Mock("content").SolidBlock(40, 30);
                    decoration.After().Mock("after").SolidBlock(30, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("before").Position(0, 0).Size(50, 20);
                        page.Mock("content").Position(0, 20).Size(50, 30);
                        page.Mock("after").Position(0, 50).Size(50, 20);
                    });
            });
    }

    [Test]
    public void RightToLeftDirection()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // the decoration is as wide as its widest slot, and is placed at the right edge of the available space
                content.ContentFromRightToLeft().Decoration(decoration =>
                {
                    decoration.Before().Mock("before").SolidBlock(50, 20);
                    decoration.Content().Mock("content").SolidBlock(40, 30);
                    decoration.After().Mock("after").SolidBlock(30, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 70)
                    .Content(page =>
                    {
                        page.Mock("before").Position(50, 0).Size(50, 20);
                        page.Mock("content").Position(50, 20).Size(50, 30);
                        page.Mock("after").Position(50, 50).Size(50, 20);
                    });
            });
    }

    [Test]
    public void BeforeSlotTallerThanAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    decoration.Before().SolidBlock(50, 150); // <-
                    decoration.Content().SolidBlock(50, 10);
                });
            })
            .ExpectLayoutException("Decoration slot (before or after) does not fit fully on the page.");
    }

    [Test]
    public void AfterSlotTallerThanAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    decoration.Content().SolidBlock(50, 10);
                    decoration.After().SolidBlock(50, 150); // <-
                });
            })
            .ExpectLayoutException("Decoration slot (before or after) does not fit fully on the page.");
    }

    [Test]
    public void ContentTallerThanAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    decoration.Before().SolidBlock(50, 20);
                    decoration.Content().SolidBlock(50, 150); // <-
                });
            })
            // the error points at the content that does not fit
            .ExpectLayoutException("The content requires more vertical space than available.");
    }

    [Test]
    public void ContentReportsMoreWidthThanAvailable()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    // simulates a child that breaks the measurement contract, built-in elements never report more space than offered
                    decoration.Content().OverflowingBlock(150, 10); // <-
                });
            })
            .ExpectLayoutException("The content slot requires more horizontal space than available.");
    }

    [Test]
    public void ContentReportsMoreHeightThanAvailable()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Decoration(decoration =>
                {
                    // simulates a child that breaks the measurement contract, built-in elements never report more space than offered
                    decoration.Before().SolidBlock(50, 20);
                    decoration.Content().OverflowingBlock(50, 150); // <-
                });
            })
            .ExpectLayoutException("The content slot requires more vertical space than available.");
    }

    [TestCase("Before")]
    [TestCase("Content")]
    [TestCase("After")]
    public void LayoutErrorNamesEachSlot(string slotName)
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // when a slot does not fit, the error shows the measurement of every slot under its name
                content.Shrink().Decoration(decoration =>
                {
                    decoration.Before().SolidBlock(50, 150); // <-
                    decoration.Content().SolidBlock(50, 10);
                    decoration.After().SolidBlock(50, 20);
                });
            })
            .ExpectLayoutException($"ElementStructure / {slotName}");
    }
}
