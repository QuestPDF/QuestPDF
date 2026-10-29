namespace QuestPDF.LayoutTests;

public class ColumnTests
{
    [Test]
    public void Typical()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 140)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").ContinuousBlock(50, 30);
                    column.Item().Mock("b").ContinuousBlock(40, 20);
                    column.Item().Mock("c").ContinuousBlock(70, 40);
                    column.Item().Mock("d").ContinuousBlock(60, 60);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(70, 140)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(70, 30);
                        page.Mock("b").Position(0, 40).Size(70, 20);
                        page.Mock("c").Position(0, 70).Size(70, 40);
                        page.Mock("d").Position(0, 120).Size(70, 20);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 40)
                    .Content(page =>
                    {
                        page.Mock("d").Position(0, 0).Size(60, 40);
                    });
            });
    }

    [Test]
    public void SingleItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Mock("a").ContinuousBlock(50, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                    });
            });
    }

    [Test]
    public void ZeroHeightItemDoesNotConsumeSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 30);
                    column.Item().Mock("b").SolidBlock(70, 0); // e.g. a fixed-width container whose content is hidden with ShowIf(false)
                    column.Item().Mock("c").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the item widens the column but gets no spacing before or after
                document
                    .Page()
                    .RequiredAreaSize(70, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(70, 30);
                        page.Mock("b").Position(0, 30).Size(70, 0);
                        page.Mock("c").Position(0, 40).Size(70, 20);
                    });
            });
    }

    [Test]
    public void NoSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(0);
                    
                    column.Item().Mock("a").ContinuousBlock(50, 30);
                    column.Item().Mock("b").ContinuousBlock(40, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(0, 30).Size(50, 20);
                    });
            });
    }

    [Test]
    public void PartialRenderItem()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 80)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(5);
                    
                    column.Item().Mock("a").ContinuousBlock(50, 40);
                    column.Item().Mock("b").ContinuousBlock(60, 50);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(60, 80)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(60, 40);
                        page.Mock("b").Position(0, 45).Size(60, 35);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 15)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(60, 15);
                    });
            });
    }

    [Test]
    public void ItemThatDoesNotFitIsMovedToNextPage()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 60);
                    column.Item().Mock("b").SolidBlock(40, 70);
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
                    .RequiredAreaSize(40, 70)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(40, 70);
                    });
            });
    }

    [Test]
    public void ItemsExactlyFillingAvailableSpace()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 40);
                    column.Item().Mock("b").SolidBlock(50, 50);
                    column.Item().Mock("c").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 40);
                        page.Mock("b").Position(0, 50).Size(50, 50);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(50, 20)
                    .Content(page =>
                    {
                        page.Mock("c").Position(0, 0).Size(50, 20);
                    });
            });
    }

    [Test]
    public void SpacingIsIncludedWhenCheckingIfItemFits()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    // the second item fits in the remaining space (20) but not together with the spacing
                    column.Item().Mock("a").SolidBlock(50, 80);
                    column.Item().Mock("b").SolidBlock(50, 15);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 80)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 80);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(50, 15)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(50, 15);
                    });
            });
    }

    [Test]
    public void ItemIsMovedToNextPageWhenRemainingSpaceIsSmallerThanSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 95);
                    column.Item().Mock("b").SolidBlock(50, 3);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 95)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 95);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(50, 3)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(50, 3);
                    });
            });
    }

    [Test]
    public void ItemsFollowingPartiallyRenderedItemAreMovedToNextPage()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").ContinuousBlock(50, 150);
                    column.Item().Mock("b").ContinuousBlock(40, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(50, 80)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 50);
                        page.Mock("b").Position(0, 60).Size(50, 20);
                    });
            });
    }

    [Test]
    public void ItemsFollowingPartiallyRenderedItemAreMovedToNextPageEvenIfTheyFit()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Column(nestedColumn =>
                    {
                        nestedColumn.Item().Mock("a").SolidBlock(50, 60);
                        nestedColumn.Item().Mock("b").SolidBlock(50, 60);
                    });

                    column.Item().Mock("c").SolidBlock(40, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the nested column renders partially and leaves 40 of space, item c would fit there
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 60);
                    });

                document
                    .Page()
                    .RequiredAreaSize(50, 90)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(50, 60);
                        page.Mock("c").Position(0, 70).Size(50, 20);
                    });
            });
    }

    [Test]
    public void PageBreakDoesNotLeaveSpacingAtTopOfNextPage()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 30);
                    column.Item().PageBreak();
                    column.Item().Mock("b").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the page break has no height, so it gets no spacing and the column ends at the first item
                document
                    .Page()
                    .RequiredAreaSize(50, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                    });

                // the already rendered page break has no height either, so the next item starts at the top of the page
                document
                    .Page()
                    .RequiredAreaSize(50, 20)
                    .Content(page =>
                    {
                        page.Mock("b").Position(0, 0).Size(50, 20);
                    });
            });
    }

    [Test]
    public void EmptyFirstItemDoesNotConsumeSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a"); // empty item, e.g. content hidden with ShowIf(false)
                    column.Item().Mock("b").ContinuousBlock(50, 30);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 0);
                        page.Mock("b").Position(0, 0).Size(50, 30);
                    });
            });
    }

    [Test]
    public void ConsecutiveAndLastEmptyItemsDoNotConsumeSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").ContinuousBlock(50, 30);
                    column.Item().Mock("b");
                    column.Item().Mock("c");
                    column.Item().Mock("d").ContinuousBlock(50, 20);
                    column.Item().Mock("e");
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(0, 30).Size(50, 0);
                        page.Mock("c").Position(0, 30).Size(50, 0);
                        page.Mock("d").Position(0, 40).Size(50, 20);
                        page.Mock("e").Position(0, 60).Size(50, 0);
                    });
            });
    }

    [Test]
    public void ItemsHiddenWithShowIfDoNotConsumeSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().ShowIf(false).SolidBlock(50, 20);
                    column.Item().Mock("a").SolidBlock(50, 30);
                    column.Item().ShowIf(_ => false).SolidBlock(50, 20);
                    column.Item().Mock("b").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(0, 40).Size(50, 20);
                    });
            });
    }

    [Test]
    public void EmptyItemDoesNotCreateNewPageWhenAvailableSpaceIsFullyOccupied()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 100);
                    column.Item().Mock("b"); // empty item, e.g. content hidden with ShowIf(false)
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 100);
                        page.Mock("b").Position(0, 100).Size(50, 0);
                    });
            });
    }

    [Test]
    public void EmptyItemDoesNotCreateNewPageWhenRemainingSpaceIsSmallerThanSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);
                    
                    column.Item().Mock("a").SolidBlock(50, 95);
                    column.Item().Mock("b"); // empty item, e.g. content hidden with ShowIf(false)
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 95)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 95);
                        page.Mock("b").Position(0, 95).Size(50, 0);
                    });
            });
    }

    [Test]
    public void ZeroHeightItemDoesNotCreateNewPageWhenRemainingSpaceIsSmallerThanSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 95);
                    column.Item().Mock("b").SolidBlock(70, 0);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the item needs no spacing, so it fits in the remaining space (5) and the column ends at the first item
                document
                    .Page()
                    .RequiredAreaSize(70, 95)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(70, 95);
                        page.Mock("b").Position(0, 95).Size(70, 0);
                    });
            });
    }

    // The tests below cover https://github.com/QuestPDF/QuestPDF/issues/1484
    // An element that postpones its content to the next page (PreventPageBreak, EnsureSpace) takes no vertical space on the current page.
    // When a parent measures or draws the column with that zero height, the column must still accept the item,
    // even if it takes horizontal space (padding). Otherwise, the layout throws an exception or never makes progress.

    [Test]
    public void PostponedItemWithHorizontalPaddingInsideRow()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().PaddingLeft(10).PreventPageBreak().Mock("a").ContinuousBlock(50, 150);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                // the content is taller than the page, PreventPageBreak postpones it once
                // TODO: known limitation, the first page stays blank. The content is already at the top of an empty page and cannot fit on any page,
                // so postponing it gains nothing. Once PreventPageBreak moves content only when the next page can offer more space,
                // the content should start on the first page and this expectation should change.
                document
                    .Page()
                    .RequiredAreaSize(100, 0)
                    .Content(page => { });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(10, 0).Size(90, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(10, 0).Size(90, 50);
                    });
            });
    }

    [Test]
    public void PostponedItemWithHorizontalPaddingInsideNestedColumn()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Column(outerColumn =>
                {
                    outerColumn.Item().Column(column =>
                    {
                        column.Item().PaddingLeft(10).EnsureSpace(120).Mock("a").ContinuousBlock(50, 150);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                // EnsureSpace requires more space (120) than available on any page, it postpones the content once
                // TODO: known limitation, the first page stays blank. The content is already at the top of an empty page and no page can offer 120,
                // so postponing it gains nothing. Once EnsureSpace moves content only when the next page can offer more space,
                // the content should start on the first page and this expectation should change.
                document
                    .Page()
                    .RequiredAreaSize(10, 0)
                    .Content(page => { });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(10, 0).Size(90, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(10, 0).Size(90, 50);
                    });
            });
    }

    [Test]
    public void PostponedItemWithHorizontalPaddingPrecededByOtherItemInsideNestedColumn()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Column(outerColumn =>
                {
                    outerColumn.Item().Column(column =>
                    {
                        column.Spacing(10);
                        
                        column.Item().Mock("a").SolidBlock(50, 30);
                        column.Item().PaddingLeft(10).PreventPageBreak().Mock("b").ContinuousBlock(50, 150);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                // the second item does not fit in the remaining space, PreventPageBreak postpones it;
                // the postponed item has no height on this page, so it gets no spacing and the column ends at the first item
                document
                    .Page()
                    .RequiredAreaSize(50, 30)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 30);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("b").Position(10, 0).Size(90, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 50)
                    .Content(page =>
                    {
                        page.Mock("b").Position(10, 0).Size(90, 50);
                    });
            });
    }

    [Test]
    public void PostponedItemWithHorizontalPaddingWhenRemainingSpaceIsSmallerThanSpacing()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Column(outerColumn =>
                {
                    outerColumn.Item().Column(column =>
                    {
                        column.Spacing(10);
                        
                        column.Item().Mock("a").SolidBlock(50, 95);
                        column.Item().PaddingLeft(10).PreventPageBreak().Mock("b").ContinuousBlock(50, 150);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                // the remaining space (5) is smaller than the spacing (10); the postponed item has no height on this page,
                // so it needs no spacing, stays on the first page, and the column ends at the first item
                document
                    .Page()
                    .RequiredAreaSize(50, 95)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100, 95);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 100)
                    .Content(page =>
                    {
                        page.Mock("b").Position(10, 0).Size(90, 100);
                    });
                
                document
                    .Page()
                    .RequiredAreaSize(60, 50)
                    .Content(page =>
                    {
                        page.Mock("b").Position(10, 0).Size(90, 50);
                    });
            });
    }

    [Test]
    public void ItemThatDoesNotFitOnAnyPage()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().SolidBlock(50, 150);
                });
            })
            .ExpectLayoutException("The available space is not sufficient for even partially rendering a single item.");
    }

    [Test]
    public void ItemReportsMoreWidthThanAvailable()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    // simulates a child that breaks the measurement contract, built-in elements never report more space than offered
                    column.Item().OverflowingBlock(150, 50);
                });
            })
            .ExpectLayoutException("The content requires more horizontal space than available.");
    }

    [Test]
    public void ItemReportsMoreHeightThanAvailable()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    // simulates a child that breaks the measurement contract, built-in elements never report more space than offered
                    column.Item().OverflowingBlock(50, 150);
                });
            })
            .ExpectLayoutException("The content requires more vertical space than available.");
    }

    [Test]
    public void FullyRenderedColumnTakesNoSpaceOnFollowingPages()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Row(row =>
                {
                    row.RelativeItem().Column(column => column.Item().Mock("a").SolidBlock(50, 30));
                    row.RelativeItem().Column(column => column.Item().Mock("b").ContinuousBlock(50, 150));
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100, 100)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(50, 0).Size(50, 100);
                    });

                // the first column is already fully rendered, so the row measures it again and it reports an empty result
                document
                    .Page()
                    .RequiredAreaSize(100, 50)
                    .Content(page =>
                    {
                        page.Mock("b").Position(50, 0).Size(50, 50);
                    });
            });
    }

    // Sizes are compared with a tolerance of Size.Epsilon, so an item that exceeds the available space by
    // no more than the tolerance (e.g. due to floating-point rounding) is still accepted instead of being moved to the next page.

    [Test]
    public void ItemWiderThanAvailableSpaceByToleranceIsAccepted()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("a").SolidBlock(100 + Size.Epsilon, 50);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(100 + Size.Epsilon, 50)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(100 + Size.Epsilon, 50);
                    });
            });
    }

    [Test]
    public void ItemTallerThanAvailableSpaceByToleranceIsAccepted()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Item().Mock("a").SolidBlock(50, 100 + Size.Epsilon);
                });
            })
            .ExpectDrawResult(document =>
            {
                document
                    .Page()
                    .RequiredAreaSize(50, 100 + Size.Epsilon)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 100 + Size.Epsilon);
                    });
            });
    }

    [Test]
    public void ItemWithHeightWithinToleranceIsTreatedAsZeroHeight()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                content.Shrink().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Mock("a").SolidBlock(50, 30);
                    column.Item().Mock("b").SolidBlock(50, Size.Epsilon);
                    column.Item().Mock("c").SolidBlock(50, 20);
                });
            })
            .ExpectDrawResult(document =>
            {
                // the item gets no spacing and does not move the following item down
                document
                    .Page()
                    .RequiredAreaSize(50, 60)
                    .Content(page =>
                    {
                        page.Mock("a").Position(0, 0).Size(50, 30);
                        page.Mock("b").Position(0, 30).Size(50, Size.Epsilon);
                        page.Mock("c").Position(0, 40).Size(50, 20);
                    });
            });
    }

    [Test]
    public void ItemsFlowAcrossColumnsOfMultiColumn()
    {
        LayoutTest
            .HavingSpaceOfSize(100, 100)
            .ForContent(content =>
            {
                // MultiColumn saves and restores the column state while it splits the content into columns
                content.MultiColumn(multiColumn =>
                {
                    multiColumn.Columns(2);
                    multiColumn.Spacing(10);

                    multiColumn.Content().Column(column =>
                    {
                        column.Spacing(10);

                        foreach (var i in Enumerable.Range(1, 8))
                            column.Item().Mock($"item{i}").SolidBlock(40, 30);
                    });
                });
            })
            .ExpectDrawResult(document =>
            {
                // each column starts with an item, without spacing above it
                document
                    .Page()
                    .RequiredAreaSize(100, 70)
                    .Content(page =>
                    {
                        page.Mock("item1").Position(0, 0).Size(45, 30);
                        page.Mock("item2").Position(0, 40).Size(45, 30);
                        page.Mock("item3").Position(55, 0).Size(45, 30);
                        page.Mock("item4").Position(55, 40).Size(45, 30);
                    });

                document
                    .Page()
                    .RequiredAreaSize(100, 70)
                    .Content(page =>
                    {
                        page.Mock("item5").Position(0, 0).Size(45, 30);
                        page.Mock("item6").Position(0, 40).Size(45, 30);
                        page.Mock("item7").Position(55, 0).Size(45, 30);
                        page.Mock("item8").Position(55, 40).Size(45, 30);
                    });
            });
    }
}
